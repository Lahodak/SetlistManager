using Microsoft.EntityFrameworkCore;
using SetlistManager.Common.Models;
using SetlistManager.API.Data.Entities;

namespace SetlistManager.API.Services;
public class SetlistsService : ISetlistsService
{
    private readonly Data.AppDbContext _dbContext;
    private readonly OrderMappingService _orderMappingService;
    public SetlistsService(Data.AppDbContext dbContext, OrderMappingService orderMappingService)
    {
        _dbContext = dbContext;
        _orderMappingService = orderMappingService;
    }

    public async Task<SetlistModel?> GetSetlistByIdAsync(int id)
    {
        var setlist = await _dbContext.Setlists
            .Include(s => s.SongsSetlists)
            .ThenInclude(s => s.Song)
            .ThenInclude(s => s.Language)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (setlist is null)
            return null;

        return await _orderMappingService.MapSongEntityToModelOrder(setlist);
    }

    public async Task<SetlistModel?> GetSetlistByNameAsync(string name)
    {
        var setlist = await _dbContext.Setlists
            .Include(s => s.SongsSetlists)
            .ThenInclude(s => s.Song)
            .FirstOrDefaultAsync(x => x.Name.Contains(name));

        if(setlist is null) 
            return null;

        return await _orderMappingService.MapSongEntityToModelOrder(setlist);
    }

    public async Task SaveSetlistAsync(SetlistModel setlistModel)
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
            SongsSetlists = []
        };

        await _dbContext.Setlists.AddAsync(_orderMappingService.MapSongModelToEntity(setlistModel, setlistToCreate));
        await _dbContext.SaveChangesAsync();

        return;
    }

    public async Task<IEnumerable<SetlistModel>?> GetAllSetlistsOfUserAsync(int userId)
    {
        var setlists = await _dbContext.Setlists
            .Include(s => s.SongsSetlists)
            .ThenInclude(s => s.Song)
            .Where(x => x.Id == userId)
            .ToListAsync();

        return setlists.Select(setlists => _orderMappingService.MapSongEntityToModelOrder(setlists).Result);
    }
    public async Task<IEnumerable<SetlistModel>?> GetAllSetlistsAsync()
    {
        var setlists = await _dbContext.Setlists
            .Include(s => s.SongsSetlists)
            .ThenInclude(s => s.Song)
            .ToListAsync();        
        
        return setlists.Select(setlists => _orderMappingService.MapSongEntityToModelOrder(setlists).Result);
    }

    public async Task EditSetlistAsync(SetlistModel setlistModel)
    {
        var setlistToBeEdited = await _dbContext.Setlists
            .Include(x => x.SongsSetlists)
            .ThenInclude(x => x.Song)
            .FirstAsync(x => x.Id == setlistModel.Id);

        setlistToBeEdited.Name = setlistModel.Name;
        setlistToBeEdited.UpdatedAt = DateTime.Now;
        setlistToBeEdited.UpdatedBy = setlistModel.CreatorId;
        
        foreach(var song in setlistToBeEdited.SongsSetlists)
        {
            song.Order = setlistModel.Songs.First(x => x.Id == song.SongId).Order;
        }

        await _dbContext.SaveChangesAsync();
    }
}