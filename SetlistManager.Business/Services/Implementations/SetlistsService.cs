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
    private readonly int _currentUserId;

    public SetlistsService(AppDbContext dbContext, ICurrentUserContext currentUserContext)
    {
        _dbContext = dbContext;
        _currentUserId = currentUserContext.UserId;
    }

    public async Task<SetlistModel> GetSetlistByIdAsync(int setlistId)
    {
        var setlist = await _dbContext.Setlists
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .Include(s => s.Owner)                    
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Language)
            .FirstOrDefaultAsync(x =>
                x.Id == setlistId &&
                (x.OwnerId == _currentUserId || x.SetlistsUsers.Any(x => x.UserId == _currentUserId)));

        if (setlist is null)
            throw new EntryNotFoundException("Setlist not found or access denied.");

        return setlist.ToModel();
    }

    public async Task<PagedResponse<SetlistModel>> GetSetlistsAsync(PagedRequest request)
    {
        var query = _dbContext.Setlists
            .Where(s =>
                (string.IsNullOrEmpty(request.Query) || s.Name.Contains(request.Query)) &&
                (s.SetlistsUsers.Any(x => x.UserId == _currentUserId) || s.OwnerId == _currentUserId));

        return await query
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(l => l.Language)
            .Include(s => s.Owner)
            .Include(s => s.SongsSetlists)
                .ThenInclude(s => s.Song)
                    .ThenInclude(s => s.Artist)
            .Include(x => x.Owner)
            .Select(s => s.ToModel())
            .ToPaginatedResultAsync(request);
    }

    public async Task GiveAccessToSetlistAsync(int setlistId, int targetId)
    {
        var setlist = await _dbContext.Setlists
            .Include(s => s.SetlistsUsers.Where(x => x.UserId == targetId))
            .FirstOrDefaultAsync(s => s.Id == setlistId && s.OwnerId == _currentUserId);

        if (setlist is null ||
            setlist.SetlistsUsers.Count != 0 ||
            (_currentUserId != setlist.OwnerId && targetId != _currentUserId))
            throw new EntryNotFoundException("Setlist not found or access denied.");

        setlist.SetlistsUsers.Add(new()
        {
            UserId = targetId,
            SetlistId = setlistId
        });

        await _dbContext.SaveChangesAsync();
    }

    public async Task CreateSetlistAsync(SetlistCreateModel createModel)
    {
        if (await _dbContext.Setlists.AnyAsync(s =>
            s.Name == createModel.Name &&
            (s.OwnerId == _currentUserId || s.SetlistsUsers.Any(x => x.UserId == _currentUserId))))
            throw new DuplicateEntryException();

        Setlist setlistToCreate = new()
        {
            Name = createModel.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            OwnerId = _currentUserId
        };

        _dbContext.Setlists.Add(createModel.MapCreateModelToEntity(setlistToCreate));
        await _dbContext.SaveChangesAsync();
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
            .FirstOrDefaultAsync(x => x.Id == setlistModel.Id && x.OwnerId == _currentUserId);

        if (setlistToBeEdited is null)
            throw new EntryNotFoundException("Setlist not found or access denied.");

        setlistToBeEdited.Name = setlistModel.Name;
        setlistToBeEdited.UpdatedAt = DateTime.UtcNow;

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

    public async Task DeleteSetlistAsync(int setlistId)
    {
        var setlistToBeDeleted = await _dbContext.Setlists
            .FirstOrDefaultAsync(x => x.Id == setlistId && x.OwnerId == _currentUserId);

        if (setlistToBeDeleted is null)
            throw new EntryNotFoundException("Entry not found or access denied.");

        _dbContext.Setlists.Remove(setlistToBeDeleted);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAccessFromUserAsync(int setlistId, int userId)
    {
        var setlistUser = await _dbContext.SetlistsUsers
            .Include(su => su.Setlist)
            .FirstOrDefaultAsync(su => su.SetlistId == setlistId && su.UserId == userId);

        if (setlistUser is null ||
            (setlistUser.Setlist.OwnerId != _currentUserId && userId != _currentUserId))
            throw new EntryNotFoundException("Entry not found or access denied.");

        _dbContext.SetlistsUsers.Remove(setlistUser);
        await _dbContext.SaveChangesAsync();
    }
}
