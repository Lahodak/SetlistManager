using Microsoft.EntityFrameworkCore;
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

    public async Task<IEnumerable<Song>> GetSongsAsync() 
        => await _dbContext.Songs
        .Include(x => x.Language)
        .Include(x => x.Artist)
        .ToListAsync();

    public async Task<Song?> GetSongByIdAsync(int songId)
        => await _dbContext.Songs
        .Include(x => x.Language)
        .Include(x => x.Artist)
        .FirstOrDefaultAsync(x => x.Id == songId);

    public async Task<IEnumerable<Song?>> GetSongByNameAsync(string name) 
        => await _dbContext.Songs
        .Where(x => x.Name.Contains(name) || x.Artist.Nick.Contains(name))
        .Take(10)
        .ToListAsync();

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