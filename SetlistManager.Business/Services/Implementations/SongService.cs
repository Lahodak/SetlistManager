using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Extentions;
using SetlistManager.Business.Mappers;
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

    public async Task<bool> TryGiveAccessToUserAsync(int songId, int targetId)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;

        var song = await _dbContext.Songs
            .Include(x => x.SongsUsers)
            .Include(x => x.Artist)
                .ThenInclude(x => x.ArtistsUsers.Where(x => x.UserId == targetId))
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == songId);

        if (song is null || song.SongsUsers.Count != 0 || (currentUserId != song.OwnerId && targetId != currentUserId))
            return false;

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
        
        return true;
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

    public async Task<bool> TryCreateSongAsync(SongCreateModel songCreateModel)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        if (await _dbContext.Songs
        .AnyAsync(x => x.Name == songCreateModel.Name 
            && x.ArtistId == songCreateModel.ArtistId
            && (x.OwnerId == userId 
            || x.SongsUsers!.Any(su => su.UserId == userId))))
            return false;

        bool isArtistPublic = false;

        if ((await _dbContext.Artists.FirstOrDefaultAsync(x => x.Id == songCreateModel.ArtistId))!.IsPublic)
            isArtistPublic = true;

        Song song = songCreateModel.ToEntity(userId, isArtistPublic);

        _dbContext.Songs.Add(song);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> TryUpdateSongAsync(int songId, SongUpdateModel updateModel)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        if (await _dbContext.Songs.
            AnyAsync(x => 
            (x.Name == updateModel.Name) 
            && (x.ArtistId == updateModel.ArtistId)
            && ((x.OwnerId == userId) 
            || x.SongsUsers
            .Any(x => x.Song.ArtistId == updateModel.ArtistId && x.UserId == userId))))
            return false;
        
        var song = await _dbContext.Songs.FirstOrDefaultAsync(x => x.Id == songId && x.OwnerId == userId);

        if (song is null || song.OwnerId != userId || song.IsPublic)
            return false;

        song.UpdateEntity(updateModel);

        await _dbContext.SaveChangesAsync();        
        return true;
    }

    public async Task<bool> TryDeleteSongAsync(int songId)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        var song = await _dbContext.Songs.FirstOrDefaultAsync(x => x.Id == songId && x.OwnerId == userId);
        
        if (song is null)
            return false;
        
        _dbContext.Songs.Remove(song);        
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> TryMakeSongPublicAsync(int songId)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        var song = await _dbContext.Songs
            .Include(x => x.Artist)
            .FirstOrDefaultAsync(x => x.Id == songId && x.OwnerId == userId);       

        if (song is null)
            return false;

        song.IsPublic = true;
        song.Artist.IsPublic = true;

        await _dbContext.SaveChangesAsync();
        return true;
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

    public async Task<PagedResponse<SongUsageStatModel>> GetMostUsedSongsAsync(StatsPagedRequest request)
    {
        var from = request.Range switch
        {
            StatsRange.Day => DateTime.UtcNow.AddDays(-1),
            StatsRange.Week => DateTime.UtcNow.AddDays(-7),
            StatsRange.Month => DateTime.UtcNow.AddMonths(-1),
            _ => DateTime.UtcNow.AddDays(-7)
        };

        var baseQuery = _dbContext.SongsSetlists
            .Where(ss => ss.CreatedAt >= from && ss.Song.IsPublic);

        var totalCount = await baseQuery
            .Select(ss => ss.SongId)
            .Distinct()
            .CountAsync();

        var items = await baseQuery
            .GroupBy(ss => new { ss.SongId, ss.Song.Name })
            .Select(g => new SongUsageStatModel
            {
                SongId = g.Key.SongId,
                Name = g.Key.Name,
                UsageCount = g.Count()
            })
            .OrderByDescending(x => x.UsageCount)
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<SongUsageStatModel>
        {
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<PagedResponse<SongUsageStatModel>> GetMostAddedToLibraryAsync(PagedRequest request)
    {
        var baseQuery = _dbContext.SongsUsers.Where(x => x.Song.IsPublic);

        var totalCount = await baseQuery
            .Select(su => su.SongId)            
            .Distinct()
            .CountAsync();

        var items = await baseQuery
            .GroupBy(su => new { su.SongId, su.Song.Name })
            .Select(g => new SongUsageStatModel
            {
                SongId = g.Key.SongId,
                Name = g.Key.Name,
                UsageCount = g.Count()
            })
            .OrderByDescending(x => x.UsageCount)
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponse<SongUsageStatModel>
        {
            TotalCount = totalCount,
            Items = items
        };
    }

    public async Task<PagedResponse<LatestSongStatModel>> GetLatestPublicSongsAsync(PagedRequest request)
    {
        var query = _dbContext.Songs
            .Where(s => s.IsPublic);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new LatestSongStatModel
            {
                SongId = s.Id,
                Name = s.Name,
                CreatedAt = s.CreatedAt,
                ArtistNick = s.Artist.Nick
            })
            .ToListAsync();

        return new PagedResponse<LatestSongStatModel>
        {
            TotalCount = totalCount,
            Items = items
        };
    }
}