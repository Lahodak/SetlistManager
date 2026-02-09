using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Extensions;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Exceptions;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services.Implementations;

public class RoomsService : IRoomsService
{
    private readonly AppDbContext _dbContext;
    private readonly IRoomCodeService _roomCodeService;
    private readonly int _currentUserId;

    public RoomsService(AppDbContext dbContext, ICurrentUserContext currentUserContext, IRoomCodeService roomCodeService)
    {
        _dbContext = dbContext;
        _roomCodeService = roomCodeService;
        _currentUserId = currentUserContext.UserId;
    }

    public async Task<RoomModel> GetRoomByIdAsync(int roomId)
    {
        var room = await _dbContext.Rooms
            .Include(x => x.Setlist)
                .ThenInclude(x => x!.SongsSetlists)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Language)
            .Include(x => x.Setlist)
                .ThenInclude(x => x!.SongsSetlists)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Artist)
            .Include(x => x.Users)
                .ThenInclude(x => x.Instrument)
            .FirstOrDefaultAsync(x => x.Id == roomId);

        if (room is null)
            throw new EntryNotFoundException();

        var model = room.ToModel();

        if (room.Setlist is null || model.Setlist is null)
            return model;

        model.Setlist = room.Setlist.ToModel();

        return model;
    }

    public async Task<RoomModel> CreateRoomAsync(RoomCreateModel createRoomModel)
    {
        Room room = new()
        {
            Name = createRoomModel.Name,
            HostId = _currentUserId,
            SetlistId = createRoomModel.SetlistModel?.Id,
            IsPublic = createRoomModel.IsPublic,
            UpdatedBy = _currentUserId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            Code = await _roomCodeService.GenerateUniqueRoomCodeAsync()
        };

        _dbContext.Rooms.Add(room);
        await _dbContext.SaveChangesAsync();

        var createdRoom = await _dbContext.Rooms
            .Include(x => x.Setlist)
                .ThenInclude(x => x!.SongsSetlists)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Language)
            .Include(x => x.Setlist)
                .ThenInclude(x => x!.SongsSetlists)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Artist)
            .Include(x => x.Users)
                .ThenInclude(x => x.Instrument)
            .FirstAsync(x => x.Id == room.Id);

        if (createdRoom.Setlist is not null)
            createdRoom.CurrentSongId = createdRoom.Setlist.SongsSetlists
                .First(x => x.Order == 1).SongId;

        await _dbContext.SaveChangesAsync();

        var roomModel = createdRoom.ToModel();

        if (createdRoom.Setlist is null || roomModel.Setlist is null)
            return roomModel;

        roomModel.Setlist = createdRoom.Setlist.ToModel();

        return roomModel;
    }

    public async Task<RoomModel> JoinRoomAsync(JoinRoomModel joinRoomModel, User user)
    {
        var room = await _dbContext.Rooms
            .Include(x => x.Setlist)
                .ThenInclude(x => x!.SongsSetlists)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Language)
            .Include(x => x.Setlist)
                .ThenInclude(x => x!.SongsSetlists)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Artist)
            .Include(x => x.Users)
                .ThenInclude(x => x.Instrument)
            .FirstOrDefaultAsync(x => x.Code == joinRoomModel.RoomCode);

        if (room is null)
            throw new EntryNotFoundException();

        room.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var roomModel = room.ToModel();

        if (room.Setlist is null || roomModel.Setlist is null)
            return roomModel;

        roomModel.Setlist = room.Setlist.ToModel();

        return roomModel;
    }

    public async Task ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel)
    {
        var room = await _dbContext.Rooms
            .Include(x => x.Setlist)
                .ThenInclude(x => x!.SongsSetlists)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Language)
            .Include(x => x.Setlist)
                .ThenInclude(x => x!.SongsSetlists)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Artist)
            .Include(x => x.Users)
                .ThenInclude(x => x.Instrument)
            .FirstOrDefaultAsync(x => x.Id == changeCurrentSongModel.RoomId);

        if (room is null)
            return;

        room.CurrentSongId = changeCurrentSongModel.NewCurrentSongId;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PagedResponse<RoomModel>> GetPublicActiveRoomsAsync(PagedRequest request)
    {
        var query = _dbContext.Rooms
            .Where(x => x.IsPublic)
            .Where(x => x.IsActive)
            .Where(x => x.Name.Contains(request.Query ?? string.Empty));

        return await query
            .Include(x => x.Setlist)
                .ThenInclude(x => x!.SongsSetlists)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Language)
            .Include(x => x.Setlist)
                .ThenInclude(x => x!.SongsSetlists)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Artist)
            .Include(x => x.Users)
                .ThenInclude(x => x.Instrument)
            .Select(x => x.ToModel())
            .ToPaginatedResultAsync(request);
    }

    public async Task<RoomModel> GetRoomByCodeAsync(string roomCode)
    {
        var room = await _dbContext.Rooms
            .Include(x => x.Setlist)
                .ThenInclude(x => x!.SongsSetlists)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Language)
            .Include(x => x.Setlist)
                .ThenInclude(x => x!.SongsSetlists)
                .ThenInclude(x => x.Song)
                .ThenInclude(x => x.Artist)
            .Include(x => x.Users)
                .ThenInclude(x => x.Instrument)
            .FirstOrDefaultAsync(x => x.Code == roomCode);

        if (room is null)
            throw new EntryNotFoundException();

        var model = room.ToModel();

        if (room.Setlist is null || model.Setlist is null)
            return model;

        model.Setlist = room.Setlist.ToModel();

        return model;
    }
}