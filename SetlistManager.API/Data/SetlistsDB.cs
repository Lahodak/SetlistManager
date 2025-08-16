using Dapper;
using Microsoft.EntityFrameworkCore;
using SetlistManager.Common.Models;
using SetlistManager.API.Data.Entities;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace SetlistManager.API.Data;
public class SetlistsDB: ISetlistsDB
{
    private readonly APIDbContext _dbContext;
    public SetlistsDB(APIDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SetlistModel?> GetSetlistByIdAsync(int id)
    {
        var setlist = await _dbContext.Setlists    
            .Include(s => s.SongsSetlists)
            .ThenInclude(s => s.Song)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (setlist is null)
            return null;

        return setlist.ToModel();
    }

    public async Task<SetlistModel?> GetSetlistByNameAsync(string name)
    {
        var setlist = await _dbContext.Setlists
            .Include(s => s.SongsSetlists)
            .ThenInclude(s => s.Song)
            .FirstOrDefaultAsync(x => x.Name.Contains(name));

        if (setlist is null)
            return null;

        return setlist.ToModel();
    }

    public async Task<int> SaveSetlistAsync(SetlistModel setlistModel)
    {

        var songs = new List<Song>();
        foreach (var x in setlistModel.Songs)
        {
            var song = await _dbContext.Songs.FirstOrDefaultAsync(y => y.Id == x.Id);
            if (song != null)
                songs.Add(song);
        }

        var setlistToCreate = new Setlist
        {
            Name = setlistModel.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Creator = await _dbContext.Users.FirstAsync(x => x.Id == setlistModel.CreatorId),
            CreatorId = setlistModel.CreatorId,
            UpdatedBy = setlistModel.CreatorId,
            SongsSetlists = songs.Select(s => new SongsSetlists { Song = s }).ToList()
        };

        await _dbContext.Setlists.AddAsync(setlistToCreate);
        await _dbContext.SaveChangesAsync();

        return setlistToCreate.Id;
    }
}