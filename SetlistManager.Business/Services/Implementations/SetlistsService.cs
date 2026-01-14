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

    public async Task<SetlistModel?> GetSetlistByIdAsync(int setlistId, int userId)
    {
        var setlist = await _dbContext.Setlists
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Language)
            .FirstOrDefaultAsync(x => x.Id == setlistId 
                && (x.OwnerId == userId) || x.SetlistsUsers.Any(x => x.UserId == userId));   

        return setlist?.ToModel();
    }

    public async Task<PagedResponse<SetlistModel>?> GetSetlistsAsync(int userId, PagedRequest request)
    {
        var query = _dbContext.Setlists
            .Where(s =>
            (string.IsNullOrEmpty(request.Query) || s.Name.Contains(request.Query))
            && (s.SetlistsUsers.Any(x => x.UserId == userId) || s.OwnerId == userId));

        var totalCount = await query.CountAsync();

        var setlists = await _dbContext.Setlists
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(l => l.Language)
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .Include(x => x.Owner)
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        PagedResponse<SetlistModel> response = new()
        {
            TotalCount = totalCount,
            Items = setlists
                .Select(setlists => setlists
                .ToModel())
                .ToList()
        };

        return response;
    }

    public async Task<bool> TryGiveAccessToSetlistAsync(int setlistId, int targetId, int currentUserId)
    {
        var setlist = await _dbContext.Setlists
            .Include(s => s.SetlistsUsers)
            .FirstOrDefaultAsync(s => s.Id == setlistId && s.OwnerId == currentUserId);

        if(setlist is null)
            return false;

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == targetId);

        if(user is null)
            return false;

        if(setlist.SetlistsUsers.Any(su => su.UserId == targetId))
            return false;

        setlist.SetlistsUsers.Add(new()
        {
            UserId = targetId,
            SetlistId = setlistId
        });

        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> TryCreateSetlistAsync(SetlistModel setlistModel, int creatorId)
    {
        if(await _dbContext.Setlists
            .AnyAsync(s => s.Name == setlistModel.Name 
            && (s.OwnerId == creatorId || s.SetlistsUsers.Any(x => x.UserId == creatorId))))        
            return false;
        
        Setlist setlistToCreate = new()
        {
            Name = setlistModel.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OwnerId = setlistModel.OwnerId
        };

        _dbContext.Setlists.Add(setlistModel.MapSongModelToEntity(setlistToCreate));
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task EditSetlistAsync(SetlistModel setlistModel, int currentUserId)
    {
        var setlistToBeEdited = await _dbContext.Setlists
            .Include(x => x.SongsSetlists)
            .ThenInclude(x => x.Song)
                .ThenInclude(l => l.Language)
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .FirstOrDefaultAsync(x => x.Id == setlistModel.Id && x.OwnerId == currentUserId);

        if (setlistToBeEdited is null)
            return;

        setlistToBeEdited.Name = setlistModel.Name;
        setlistToBeEdited.UpdatedAt = DateTime.Now;

        var existingSongIds = setlistToBeEdited.SongsSetlists
            .Select(s => s.SongId)
            .ToList();

        var newSongIds = setlistModel.Songs
            .Select(s => s.Id)
            .ToList();

        var songsToRemove = setlistToBeEdited.SongsSetlists
            .Where(s => !newSongIds.Contains(s.SongId))
            .ToList();

        _dbContext.SongsSetlists.RemoveRange(songsToRemove);

        var songsToAdd = newSongIds
            .Where(id => !existingSongIds.Contains(id))
            .ToList();

        foreach (var song in setlistModel.Songs.Where(s => songsToAdd.Contains(s.Id)))
        {
            _dbContext.SongsSetlists.Add(new()
            {
                SetlistId = setlistToBeEdited.Id,
                SongId = song.Id,
                Order = song.Order
            });
        }

        foreach (var song in setlistToBeEdited.SongsSetlists.Where(s => newSongIds.Contains(s.SongId)))
        {
            song.Order = setlistModel.Songs
                .First(x => x.Id == song.SongId).Order;
        }

        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> TryDeleteSetlistAsync(int setlistId, int currentUserId)
    {
        var setlistToBeDeleted = await _dbContext.Setlists
            .FirstOrDefaultAsync(x => x.Id == setlistId && x.OwnerId == currentUserId);

        if (setlistToBeDeleted is null)
            return false;
        
        _dbContext.Setlists.Remove(setlistToBeDeleted);
        await _dbContext.SaveChangesAsync();
        
        return true;
    }
}