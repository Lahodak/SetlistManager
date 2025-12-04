using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services.Implementations;
public class SetlistsService : ISetlistsService
{
    private readonly AppDbContext _dbContext;

    public SetlistsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
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

        return setlist.MapSongEntityToModelWithOrder();
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

        await _dbContext.Setlists.AddAsync(setlistModel.MapSongModelToEntity(setlistToCreate));
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

        return setlists.Select(setlists => setlists.MapSongEntityToModelWithOrder());
    }

    public async Task<PagedResponse<SetlistModel>> GetAllSetlistsAsync(PagedRequest request)
    {
        var query = _dbContext.Setlists
            .Where(s => string.IsNullOrEmpty(request.Query) || s.Name.Contains(request.Query));

        var totalCount = await query.CountAsync();

        var setlists = await query
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(l => l.Language)
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        PagedResponse<SetlistModel> response = new()
        {
            TotalCount = totalCount,
            Items = setlists
                .Select(setlists => setlists
                .MapSongEntityToModelWithOrder())
                .ToList()
        };

        return response;
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

        foreach (var song in setlistToBeEdited.SongsSetlists)
        {
            song.Order = setlistModel.Songs.First(x => x.Id == song.SongId).Order;
        }

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