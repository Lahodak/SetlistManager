using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services.Implementations;

public class SongService : ISongService
{
    private readonly AppDbContext _dbContext;

    public SongService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResponse<SongModel>> GetSongsAsync(PagedRequest request, int userId)
    {
        var searchQuery = request.Query ?? string.Empty;
        var query = _dbContext.Songs
            .Where(x => x.Name.Contains(searchQuery) || x.Artist.Nick.Contains(searchQuery));

        query = request.ContentType == ContentType.Private
            ? query.Where(x => x.OwnerId == userId || x.SongsUsers.Any(su => su.UserId == userId))
            : query.Where(x => x.IsPublic);

        var totalCount = await query.CountAsync();

        var songs = await query
            .Include(x => x.Language)
            .Include(x => x.Artist)
            .Include(x => x.Owner)
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        return new PagedResponse<SongModel>
        {
            TotalCount = totalCount,
            Items = songs
                .Select(x => x.ToModel())
                .ToList()
        };
    }

    public async Task<bool> TryGiveAccessToUserAsync(int songId, int targetId, int currentUserId)
    {
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

    public async Task<SongModel?> GetSongByIdAsync(int songId, int userId)
    {
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

    public async Task<bool> TryCreateSongAsync(SongCreateModel songCreateModel, int userId)
    {
        if (await _dbContext.Songs
        .AnyAsync(x => x.Name == songCreateModel.Name 
            && x.ArtistId == songCreateModel.ArtistId
            && (x.OwnerId == userId 
            || x.SongsUsers!.Any(su => su.UserId == userId))))
            return false;

        bool isArtistPublic = false;

        if ((await _dbContext.Artists.FirstOrDefaultAsync(x => x.Id == songCreateModel.ArtistId))!.IsPublic)
            isArtistPublic = true;

        Song song = new()
        {
            Name = songCreateModel.Name,
            ArtistId = songCreateModel.ArtistId,
            TabsURL = songCreateModel.TabsURL,
            AudioURL = songCreateModel.AudioURL,
            Key = songCreateModel.Key,
            Tuning = songCreateModel.Tuning,
            BPM = songCreateModel.BPM!.Value,
            CreatedAt = DateTime.UtcNow,            
            OwnerId = userId,
            LanguageId = songCreateModel.LanguageId,
            IsPublic = isArtistPublic
        };

        _dbContext.Songs.Add(song);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> TryUpdateSongAsync(int songId, SongUpdateModel updateModel, int userId)
    {
        if (await _dbContext.Songs.AnyAsync(x => (x.Name == updateModel.Name) && (x.ArtistId == updateModel.ArtistId)
        && ((x.OwnerId == userId) || x.SongsUsers.Any(x => x.Song.ArtistId == updateModel.ArtistId && x.UserId == userId))))
            return false;
        
        var song = await _dbContext.Songs.FirstOrDefaultAsync(x => x.Id == songId && x.OwnerId == userId);

        if (song is null || song.OwnerId != userId || song.IsPublic)
            return false;

        song.Name = updateModel.Name;
        song.ArtistId = updateModel.ArtistId;
        song.TabsURL = updateModel.TabsURL;
        song.AudioURL = updateModel.AudioURL;
        song.Key = updateModel.Key;
        song.Tuning = updateModel.Tuning;
        song.BPM = updateModel.BPM;
        song.LanguageId = updateModel.LanguageId;
        song.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();        
        return true;
    }

    public async Task<bool> TryDeleteSongAsync(int songId, int userId)
    {
        var song = await _dbContext.Songs.FirstOrDefaultAsync(x => x.Id == songId && x.OwnerId == userId);
        
        if (song is null)
            return false;
        
        _dbContext.Songs.Remove(song);        
        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> TryMakeSongPublicAsync(int songId, int userId)
    {
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
}