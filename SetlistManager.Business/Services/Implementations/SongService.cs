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

    public async Task<List<SongModel>?> GetSongsAsync()
    {
        var songs = await _dbContext.Songs
        .Include(x => x.Language)
        .Include(x => x.Artist)
        .ToListAsync();

        return songs
            .Select(x => x.ToModel())
            .ToList();
    }

    public async Task<SongModel?> GetSongByIdAsync(int songId)
    {
        var song = await _dbContext.Songs
        .Include(x => x.Language)
        .Include(x => x.Artist)
        .FirstOrDefaultAsync(x => x.Id == songId);

        if (song is null)
            return null;

        return song.ToModel();
    }

    public async Task UploadSongAsync(SongCreateModel songCreateModel, int userId)
    {
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
            UpdatedBy = userId,
            LanguageId = songCreateModel.LanguageId,
        };

        await _dbContext.Songs.AddAsync(song);
        await _dbContext.SaveChangesAsync();
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
        song.UpdatedBy = userId;
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