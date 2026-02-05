using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Models;
using SetlistManager.Common.Exceptions;
using SetlistManager.Data;
using SetlistManager.Data.Entities;
using SetlistManager.Business.Extensions;

namespace SetlistManager.Business.Services.Implementations;

public class SetlistsService : ISetlistsService
{
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserContext _currentUserContext;

    public SetlistsService(AppDbContext dbContext, ICurrentUserContext currentUserContext)
    {
        _dbContext = dbContext;
        _currentUserContext = currentUserContext;
    }

    public async Task<SetlistModel> GetSetlistByIdAsync(int setlistId)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        var setlist = await _dbContext.Setlists
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Language)
            .FirstOrDefaultAsync(x => x.Id == setlistId 
                && (x.OwnerId == userId) || x.SetlistsUsers.Any(x => x.UserId == userId));   

        if(setlist is null)
            throw new EntryNotFoundException("Setlist not found or access denied.");

        return setlist.ToModel();
    }

    public async Task<PagedResponse<SetlistModel>> GetSetlistsAsync(PagedRequest request)
    {
        int userId = _currentUserContext.GetCurrentUserId()!.Value;

        var query = _dbContext.Setlists
            .Where(s =>
            (string.IsNullOrEmpty(request.Query) || s.Name.Contains(request.Query))
            && (s.SetlistsUsers.Any(x => x.UserId == userId) || s.OwnerId == userId));

        var result = await query
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(l => l.Language)
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .Include(x => x.Owner)
            .AsNoTracking()
            .ToPaginatedResultAsync(request);

        return new PagedResponse<SetlistModel>
        {
            TotalCount = result.TotalCount,
            Items = result.Items
                .Select(s => s.ToModel())
                .ToList()
        };
    }

    public async Task TryGiveAccessToSetlistAsync(int setlistId, int targetId)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;

        var setlist = await _dbContext.Setlists
            .Include(s => s.SetlistsUsers)
            .FirstOrDefaultAsync(s => s.Id == setlistId && s.OwnerId == currentUserId);

        if (setlist is null || setlist.SetlistsUsers.Count != 0 || (currentUserId != setlist.OwnerId && targetId != currentUserId))
            throw new EntryNotFoundException("Setlist not found or access denied.");

        setlist.SetlistsUsers.Add(new()
        {
            UserId = targetId,
            SetlistId = setlistId
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task TryCreateSetlistAsync(SetlistModel setlistModel)
    {
        int creatorId = _currentUserContext.GetCurrentUserId()!.Value;

        if (await _dbContext.Setlists
            .AnyAsync(s => s.Name == setlistModel.Name 
            && (s.OwnerId == creatorId || s.SetlistsUsers.Any(x => x.UserId == creatorId))))        
            throw new DuplicateEntryException();
        
        Setlist setlistToCreate = new()
        {
            Name = setlistModel.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OwnerId = setlistModel.OwnerId
        };

        _dbContext.Setlists.Add(setlistModel.MapSongModelToEntity(setlistToCreate));
        await _dbContext.SaveChangesAsync();
    }

    public async Task EditSetlistAsync(SetlistModel setlistModel)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;

        var setlistToBeEdited = await _dbContext.Setlists
            .Include(x => x.SongsSetlists)
            .ThenInclude(x => x.Song)
                .ThenInclude(l => l.Language)
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .FirstOrDefaultAsync(x => x.Id == setlistModel.Id && x.OwnerId == currentUserId);

        if (setlistToBeEdited is null)
            throw new EntryNotFoundException("Setlist not found or access denied.");

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

    public async Task TryDeleteSetlistAsync(int setlistId)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;

        var setlistToBeDeleted = await _dbContext.Setlists
            .FirstOrDefaultAsync(x => x.Id == setlistId && x.OwnerId == currentUserId);

        if (setlistToBeDeleted is null)
            throw new EntryNotFoundException("Entry not found or access denied.");
        
        _dbContext.Setlists.Remove(setlistToBeDeleted);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAccessFromUserAsync(int setlistId, int userId)
    {
        int currentUserId = _currentUserContext.GetCurrentUserId()!.Value;

        var setlistUser = await _dbContext.SetlistsUsers
            .Include(su => su.Setlist)
            .FirstOrDefaultAsync(su => su.SetlistId == setlistId && su.UserId == userId);

        if (setlistUser is null || (setlistUser.Setlist.OwnerId != currentUserId && userId != currentUserId))
            throw new EntryNotFoundException("Entry not found or access denied.");

        _dbContext.SetlistsUsers.Remove(setlistUser);
        await _dbContext.SaveChangesAsync();
    }
}