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

    public async Task<PagedResponse<SongModel>> GetSongsAsync(PagedRequest request)
    {
        var query = _dbContext.Songs
        .Where(x => x.Name.Contains(request.Query ?? string.Empty) || x.Artist.Nick.Contains(request.Query ?? string.Empty));
        
        var totalCount = await query.CountAsync();

        var songs = await query
            .Include(x => x.Language)
            .Include(x => x.Artist)
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        PagedResponse<SongModel> response = new()
        {
            TotalCount = totalCount,
            Items = songs
                .Select(x => x.ToModel())
                .ToList()
        };

        return response;
    }

    public async Task<SongModel?> GetSongByIdAsync(int songId)
    {
        var song = await _dbContext.Songs
        .Include(x => x.Language)
        .Include(x => x.Artist)
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == songId);

        if (song is null)
            return null;

        return song.ToModel();
    }

    public async Task<bool> TrySaveSongAsync(SongCreateModel songCreateModel, int userId)
    {
        if(await _dbContext.Songs.AnyAsync(x => x.Name == songCreateModel.Name && x.ArtistId == songCreateModel.ArtistId))
            return false;

        Song song = new()
        {
            Name = songCreateModel.Name,
            ArtistId = songCreateModel.ArtistId,
            TabsURL = songCreateModel.TabsURL,
            AudioURL = songCreateModel.AudioURL,
            Key = songCreateModel.Key,
            Tuning = songCreateModel.Tuning,
            BPM = songCreateModel.BPM,
            CreatedAt = DateTime.UtcNow,
            OwnerId = userId,
            LanguageId = songCreateModel.LanguageId,
        };

        await _dbContext.Songs.AddAsync(song);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> TryUpdateSongAsync(int songId, SongUpdateModel updateModel, int userId)
    {
        var song = await _dbContext.Songs.FirstOrDefaultAsync(x => x.Id == songId);

        if (song is null)
            return false;

        song.Name = updateModel.Name;
        song.ArtistId = updateModel.ArtistId;
        song.TabsURL = updateModel.TabsURL;
        song.AudioURL = updateModel.AudioURL;
        song.Key = updateModel.Key;
        song.Tuning = updateModel.Tuning;
        song.BPM = updateModel.BPM;
        song.OwnerId = userId;
        song.LanguageId = updateModel.LanguageId;
        song.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> TryDeleteSongAsync(int songId)
    {
        var song = await _dbContext.Songs.FirstOrDefaultAsync(x => x.Id == songId);
        
        if (song is null)
            return false;
        
        _dbContext.Songs.Remove(song);        
        await _dbContext.SaveChangesAsync();
        
        return true;
    }
}