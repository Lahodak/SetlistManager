using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Extensions;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Exceptions;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services.Implementations;

public class SongService : ISongService
{
    private readonly AppDbContext _dbContext;
    private readonly int _currentUserId;

    public SongService(AppDbContext dbContext, ICurrentUserContext currentUserContext)
    {
        _dbContext = dbContext;
        _currentUserId = currentUserContext.UserId;
    }

    public async Task<PagedResponse<SongModel>> GetSongsAsync(ContentPagedRequest request)
    {
        var searchQuery = request.Query ?? string.Empty;
        var query = _dbContext.Songs
            .Where(x => x.Name.Contains(searchQuery) || x.Artist.Nick.Contains(searchQuery));

        query = request.ContentType == ContentType.Private
            ? query.Where(x => x.OwnerId == _currentUserId || x.SongsUsers.Any(su => su.UserId == _currentUserId))
            : query.Where(x => x.IsPublic);

        return await query
            .Include(x => x.Language)
            .Include(x => x.Artist)
            .Include(x => x.Owner)
            .Select(x => x.ToModel())
            .ToPaginatedResultAsync(request);
    }

    public async Task GiveAccessToUserAsync(int songId, int targetId)
    {
        var song = await _dbContext.Songs
            .Include(x => x.SongsUsers.Where(x => x.UserId == targetId))
            .Include(x => x.Artist)
                .ThenInclude(x => x.ArtistsUsers.Where(x => x.UserId == targetId))
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == songId);

        if (song is null || song.SongsUsers.Count != 0 || (_currentUserId != song.OwnerId && targetId != _currentUserId))
            throw new EntryNotFoundException();

        SongsUsers songsUsers = new()
        {
            SongId = songId,
            UserId = targetId
        };

        if (song.Artist.ArtistsUsers.Count == 0 || song.Artist.OwnerId == targetId)
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
        var song = await _dbContext.Songs
            .Include(x => x.Language)
            .Include(x => x.Artist)
            .Include(x => x.Owner)
            .Include(x => x.SongsUsers)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == songId);

        if (song is null || (!song.IsPublic && !(song.OwnerId == _currentUserId || song.SongsUsers.Any(x => x.UserId == _currentUserId))))
            return null;

        return song.ToModel();
    }

    public async Task CreateSongAsync(SongCreateModel songCreateModel)
    {
        if (await _dbContext.Songs.AnyAsync(x =>
            x.Name == songCreateModel.Name &&
            x.ArtistId == songCreateModel.ArtistId &&
            (x.OwnerId == _currentUserId || x.SongsUsers!.Any(su => su.UserId == _currentUserId))))
            throw new DuplicateEntryException();

        bool isArtistPublic = (await _dbContext.Artists.FirstOrDefaultAsync(x => x.Id == songCreateModel.ArtistId))!.IsPublic;

        Song song = songCreateModel.ToEntity(_currentUserId, isArtistPublic);

        _dbContext.Songs.Add(song);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateSongAsync(int songId, SongUpdateModel updateModel)
    {
        var song = await _dbContext.Songs.FirstOrDefaultAsync(x => x.Id == songId && x.OwnerId == _currentUserId);

        if (song is null || song.IsPublic)
            throw new EntryNotFoundException();

        if (await _dbContext.Songs.AnyAsync(x =>
            x.Id != songId &&
            x.Name == updateModel.Name &&
            x.ArtistId == updateModel.ArtistId &&
            (x.OwnerId == _currentUserId ||
             x.SongsUsers.Any(su => su.SongId == x.Id && su.UserId == _currentUserId))))
            throw new DuplicateEntryException();

        song.UpdateEntity(updateModel);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteSongAsync(int songId)
    {
        var song = await _dbContext.Songs.FirstOrDefaultAsync(x => x.Id == songId && x.OwnerId == _currentUserId);

        if (song is null)
            throw new EntryNotFoundException();

        _dbContext.Songs.Remove(song);
        await _dbContext.SaveChangesAsync();
    }

    public async Task MakeSongPublicAsync(int songId)
    {
        var song = await _dbContext.Songs
            .Include(x => x.Artist)
            .FirstOrDefaultAsync(x => x.Id == songId && x.OwnerId == _currentUserId);

        if (song is null)
            throw new EntryNotFoundException();

        song.IsPublic = true;
        song.Artist.IsPublic = true;

        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAccessFromUserAsync(int songId, int userId)
    {
        var songUser = await _dbContext.SongsUsers
            .Include(x => x.Song)
                .ThenInclude(x => x.Artist)
                    .ThenInclude(x => x.ArtistsUsers.Where(x => x.UserId == userId))
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.SongId == songId && x.UserId == userId);

        if (songUser is null || (songUser.Song.OwnerId != _currentUserId && userId != _currentUserId))
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

    private static DateTime GetDateFromForStats(StatsRange range) =>
        range switch
        {
            StatsRange.Day => DateTime.UtcNow.AddDays(-1),
            StatsRange.Week => DateTime.UtcNow.AddDays(-7),
            StatsRange.Month => DateTime.UtcNow.AddMonths(-1),
            _ => DateTime.MinValue
        };

    private async Task<List<SongUsageStatModel>> GetMostUsedStatsAsync(StatsRequest request)
    {
        var from = GetDateFromForStats(request.Range!.Value);

        return await _dbContext.SongsSetlists
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
    }

    private async Task<List<SongUsageStatModel>> GetMostAddedStatsAsync(StatsRequest request)
    {
        var from = GetDateFromForStats(request.Range!.Value);

        return await _dbContext.Songs
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
    }

    private async Task<List<SongUsageStatModel>> GetLatestPublicStatsAsync(StatsRequest request)
    {
        return await _dbContext.Songs
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
    }
}