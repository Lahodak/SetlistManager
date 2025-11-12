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

    public async Task<List<SongModel>?> GetSongByNameAsync(string name)
    {
        var songs = await _dbContext.Songs
        .Include(x => x.Language)
        .Include(x => x.Artist)
        .Where(x => x.Name.Contains(name) || x.Artist.Nick.Contains(name))
        .ToListAsync();

        if (songs is null)
            return null;

        return songs
            .Select(x => x.ToModel())
            .ToList();
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
            UpdatedAt = null,
            UpdatedBy = userId,
            LanguageId = songCreateModel.LanguageId,
        };

        await _dbContext.Songs.AddAsync(song);
        await _dbContext.SaveChangesAsync();
    }
}