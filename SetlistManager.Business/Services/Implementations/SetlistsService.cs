using Microsoft.EntityFrameworkCore;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services.Implementations;
public class SetlistsService : ISetlistsService
{
    private readonly AppDbContext _dbContext;
    private readonly IOrderMappingService _orderMappingService;

    public SetlistsService(AppDbContext dbContext, IOrderMappingService orderMappingService)
    {
        _dbContext = dbContext;
        _orderMappingService = orderMappingService;
    }

    public async Task<SetlistModel?> GetSetlistByIdAsync(int id)
    {
        var setlist = await _dbContext.Setlists
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Language)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (setlist is null)
            return null;

        return await _orderMappingService.MapSongEntityToModelOrder(setlist);
    }

    public async Task SaveSetlistAsync(SetlistModel setlistModel)
    {
        var setlistToCreate = new Setlist
        {
            Name = setlistModel.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatorId = setlistModel.CreatorId
        };

        await _dbContext.Setlists.AddAsync(_orderMappingService.MapSongModelToEntity(setlistModel, setlistToCreate));
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<SetlistModel>?> GetAllSetlistsOfUserAsync(int userId)
    {
        var setlists = await _dbContext.Setlists
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(l => l.Language)
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .Where(x => x.Id == userId)
            .ToListAsync();

        return setlists.Select(setlists => _orderMappingService.MapSongEntityToModelOrder(setlists).Result);
    }

    public async Task<IEnumerable<SetlistModel>?> GetAllSetlistsAsync()
    {
        var setlists = await _dbContext.Setlists
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(l => l.Language)
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .ToListAsync();        
        
        return setlists
            .Select(setlists => _orderMappingService
            .MapSongEntityToModelOrder(setlists).Result);
    }

    public async Task EditSetlistAsync(SetlistModel setlistModel)
    {
        var setlistToBeEdited = await _dbContext.Setlists
            .Include(x => x.SongsSetlists)
            .ThenInclude(x => x.Song)
                .ThenInclude(l => l.Language)
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .FirstAsync(x => x.Id == setlistModel.Id);

        setlistToBeEdited.Name = setlistModel.Name;
        setlistToBeEdited.UpdatedAt = DateTime.Now;

        setlistToBeEdited.SongsSetlists
            .ToList()
            .ForEach(s => s.Order = setlistModel.Songs.First(x => x.Id == s.SongId).Order);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> TryDeleteSetlistAsync(int id)
    {
        var setlistToBeDeleted = await _dbContext.Setlists
            .FirstOrDefaultAsync(x => x.Id == id);

        if (setlistToBeDeleted is null)
            return false;
        
        _dbContext.Setlists.Remove(setlistToBeDeleted);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }
}