using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Extentions;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Exceptions;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services.Implementations;

public class SongService : ISongService
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserContext _currentUserContext;

    public SongService(AppDbContext dbContext, ICurrentUserContext currentUserContext)
    {
        _dbContext = dbContext;
        _currentUserContext = currentUserContext;
    }

    public async Task<PagedResponse<SongModel>> GetSongsAsync(PagedRequest request)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        var searchQuery = request.Query ?? string.Empty;
        var query = _dbContext.Songs
            .Where(x => x.Name.Contains(searchQuery) || x.Artist.Nick.Contains(searchQuery));

        query = request.ContentType == ContentType.Private
            ? query.Where(x => x.OwnerId == userId || x.SongsUsers.Any(su => su.UserId == userId))
            : query.Where(x => x.IsPublic);

        var result = await query
            .Include(x => x.Language)
            .Include(x => x.Artist)
            .Include(x => x.Owner)
            .AsNoTracking()
            .ToPaginatedResultAsync(request);

        return new PagedResponse<SongModel>
        {
            TotalCount = result.TotalCount,
            Items = result.Items
                .Select(x => x.ToModel())
                .ToList()
        };
    }

    public async Task TryGiveAccessToUserAsync(int songId, int targetId)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;

        var song = await _dbContext.Songs
            .Include(x => x.SongsUsers)
            .Include(x => x.Artist)
                .ThenInclude(x => x.ArtistsUsers.Where(x => x.UserId == targetId))
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == songId);

        if (song is null || song.SongsUsers.Count != 0 || (currentUserId != song.OwnerId && targetId != currentUserId))
            throw new EntryNotFoundException();

        SongsUsers songsUsers = new()
        {
            SongId = songId,
            UserId = targetId
        };

        if(song.Artist.ArtistsUsers.Count == 0 || song.Artist.OwnerId == targetId)
        {
            ArtistsUsers artistsUsers = new()
            {
                ArtistId = song.ArtistId,
                UserId = targetId
            };

            _dbContext.ArtistsUsers.Add(artistsUsers);
        }
        
        _dbContext.SongsUsers.Add(songsUsers);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<SongModel?> GetSongByIdAsync(int songId)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        var song = await _dbContext.Songs
        .Include(x => x.Language)
        .Include(x => x.Artist)
        .Include(x => x.Owner)
        .Include(x => x.SongsUsers)
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == songId);               

        if (song is null || (!song.IsPublic && !(song.OwnerId == userId || song.SongsUsers.Any(x => x.UserId == userId))))
            return null;
        
        return song.ToModel();
    }

    public async Task TryCreateSongAsync(SongCreateModel songCreateModel)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        if (await _dbContext.Songs
        .AnyAsync(x => x.Name == songCreateModel.Name
            && x.ArtistId == songCreateModel.ArtistId
            && (x.OwnerId == userId
            || x.SongsUsers!.Any(su => su.UserId == userId))))
            throw new DuplicateEntryException();

        bool isArtistPublic = false;

        if ((await _dbContext.Artists.FirstOrDefaultAsync(x => x.Id == songCreateModel.ArtistId))!.IsPublic)
            isArtistPublic = true;

        Song song = songCreateModel.ToEntity(userId, isArtistPublic);

        _dbContext.Songs.Add(song);
        await _dbContext.SaveChangesAsync();
    }

    public async Task TryUpdateSongAsync(int songId, SongUpdateModel updateModel)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        if (await _dbContext.Songs.
            AnyAsync(x => 
            (x.Name == updateModel.Name) 
            && (x.ArtistId == updateModel.ArtistId)
            && ((x.OwnerId == userId) 
            || x.SongsUsers
            .Any(x => x.Song.ArtistId == updateModel.ArtistId && x.UserId == userId))))
            throw new DuplicateEntryException();

        var song = await _dbContext.Songs.FirstOrDefaultAsync(x => x.Id == songId && x.OwnerId == userId);

        if (song is null || song.OwnerId != userId || song.IsPublic)
            throw new EntryNotFoundException();

        song.UpdateEntity(updateModel);

        await _dbContext.SaveChangesAsync();        
    }

    public async Task TryDeleteSongAsync(int songId)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        var song = await _dbContext.Songs.FirstOrDefaultAsync(x => x.Id == songId && x.OwnerId == userId);

        if (song is null)
            throw new EntryNotFoundException();
        
        _dbContext.Songs.Remove(song);        
        await _dbContext.SaveChangesAsync();
    }

    public async Task TryMakeSongPublicAsync(int songId)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        var song = await _dbContext.Songs
            .Include(x => x.Artist)
            .FirstOrDefaultAsync(x => x.Id == songId && x.OwnerId == userId);       

        if (song is null)
            throw new EntryNotFoundException();

        song.IsPublic = true;
        song.Artist.IsPublic = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAccessFromUserAsync(int songId, int userId)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;

        var songUser = await _dbContext.SongsUsers
            .Include(x => x.Song)
                .ThenInclude(x => x.Artist)
                    .ThenInclude(x => x.ArtistsUsers.Where(x => x.UserId == userId))
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.SongId == songId && x.UserId == userId);

        if (songUser is null || (songUser.Song.OwnerId != currentUserId && userId != currentUserId))
            return;

        _dbContext.SongsUsers.Remove(songUser);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<SongUsageStatModel>> GetStatisticsAsync(StatsRequest request)
    {
        return request.Metric switch
        {
            StatsMetric.MostUsed => await GetMostUsedStatsAsync(request),
            StatsMetric.MostAdded => await GetMostAddedStatsAsync(request),
            StatsMetric.LatestPublic => await GetLatestPublicStatsAsync(request),
            _ => []
        };
    }

    private async Task<List<SongUsageStatModel>> GetMostUsedStatsAsync(StatsRequest request)
    {
        var from = GetDateFromForStats(request.Range!.Value);

        var result = await _dbContext.SongsSetlists
            .Where(ss => ss.CreatedAt >= from && ss.Song.IsPublic)
            .GroupBy(ss => ss.SongId)
            .Select(g => new SongUsageStatModel
            {
                SongId = g.Key,
                Name = g.First().Song.Name,
                Artist = g.First().Song.Artist.Nick,
                UsageCount = g.Count(),
            })
            .OrderByDescending(s => s.UsageCount)
            .Take(request.Limit!.Value)
            .AsNoTracking()
            .ToListAsync();

        return result;
    }

    private async Task<List<SongUsageStatModel>> GetMostAddedStatsAsync(StatsRequest request)
    {
        var from = GetDateFromForStats(request.Range!.Value);

        var result = await _dbContext.Songs
            .Where(s => s.CreatedAt >= from && s.IsPublic)
            .Select(s => new SongUsageStatModel
            {
                SongId = s.Id,
                Name = s.Name,
                Artist = s.Artist.Nick,
                UsageCount = s.SongsSetlists.Count(ss => ss.CreatedAt >= from),
            })
            .OrderByDescending(s => s.UsageCount)
            .Take(request.Limit!.Value)
            .AsNoTracking()
            .ToListAsync();

        return result;
    }

    private async Task<List<SongUsageStatModel>> GetLatestPublicStatsAsync(StatsRequest request)
    {
        var result = await _dbContext.Songs
            .Where(s => s.IsPublic)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new SongUsageStatModel
            {
                SongId = s.Id,
                Name = s.Name,
                Artist = s.Artist.Nick
            })
            .Take(request.Limit!.Value)
            .AsNoTracking()
            .ToListAsync();

        return result;
    }

    private static DateTime GetDateFromForStats(StatsRange range)
    {
        return range switch
        {
            StatsRange.Day => DateTime.UtcNow.AddDays(-1),
            StatsRange.Week => DateTime.UtcNow.AddDays(-7),
            StatsRange.Month => DateTime.UtcNow.AddMonths(-1),
            _ => DateTime.MinValue
        };
    }
}