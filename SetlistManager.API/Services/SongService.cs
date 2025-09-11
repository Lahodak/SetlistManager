using Microsoft.EntityFrameworkCore;
using SetlistManager.API.Data.Entities;

namespace SetlistManager.API.Services;

public class SongService : ISongService
{
    private readonly Data.AppDbContext _dbContext;

    public SongService(Data.AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Song>> GetSongsAsync() 
        => await _dbContext.Songs
        .Include(x => x.Language)
        .ToListAsync();

    public async Task<Song?> GetSongByIdAsync(int id) 
        => await _dbContext.Songs.FirstOrDefaultAsync(x => x.Id == id) 
        ?? throw new Exception("Song wasn't found");

    public async Task<IEnumerable<Song?>> GetSongByNameAsync(string name) 
        => await _dbContext.Songs
        .Where(x => x.Name.Contains(name) || x.Artist.Contains(name))
        .Take(10)
        .ToListAsync();

    public async Task UploadSongAsync(Song song)
    {
        await _dbContext.Songs.AddAsync(song);
        await _dbContext.SaveChangesAsync();
    }
}