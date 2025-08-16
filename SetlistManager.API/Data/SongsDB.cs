using Dapper;
using Microsoft.EntityFrameworkCore;
using SetlistManager.API;
using SetlistManager.API.Data.Entities;
using SetlistManager.Common.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SetlistManager.API.Data;

public class SongsDB : ISongsDB
{
    private readonly APIDbContext _dbContext;

    public SongsDB(APIDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Song>> GetSongsAsync()
    {       
        return await _dbContext.Songs.ToListAsync();
    }

    public async Task<Song?> GetSongByIdAsync(int id)
    {
        return await _dbContext.Songs.FirstOrDefaultAsync(x => x.Id == id) ?? throw new Exception("Song wasn't found");
    }

    public async Task<IEnumerable<Song?>> GetSongByNameAsync(string name)
    {
        return await _dbContext.Songs
            .Where(x => x.Name.Contains(name) || x.Artist.Contains(name))
            .Take(10)
            .ToListAsync();
    }

    public async Task UploadSong(Song song)
    {
        await _dbContext.Songs.AddAsync(song);
        await _dbContext.SaveChangesAsync();
    }
}