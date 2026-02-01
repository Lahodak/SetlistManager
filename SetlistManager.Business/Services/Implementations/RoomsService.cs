using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;
using System.Text;

namespace SetlistManager.Business.Services.Implementations;

public class RoomsService : IRoomsService
{
    private const string roomCodeAvailableCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int roomCodeLength = 6;
    private readonly AppDbContext _dbContext;
    private readonly ICurrentUserContext _currentUserContext;

    public RoomsService(AppDbContext dbContext, ICurrentUserContext currentUserContext)
    {
        _dbContext = dbContext;
        _currentUserContext = currentUserContext;
    }

    public async Task<RoomModel?> GetRoomByIdAsync(int roomId)
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
            return null;

        var model = room.ToModel();

        if (room.Setlist is null || model.Setlist is null)
            return model;

        model.Setlist = room.Setlist.ToModel();

        return model;
    }

    public async Task<RoomModel> CreateRoomAsync(RoomCreateModel createRoomModel)
    {
        int hostId = _currentUserContext.GetCurrentUserId()!.Value;

        StringBuilder code = new(roomCodeLength);

        do
        {
            code.Clear();
            for (int i = 0; i < roomCodeLength; i++)
            {
                int index = Random.Shared.Next(roomCodeAvailableCharacters.Length - 1);
                code.Append(roomCodeAvailableCharacters[index]);
            }
        } 
        while (await _dbContext.Rooms.AnyAsync(x => x.Code == code.ToString()));

        Room room = new()
        {
            Name = createRoomModel.Name,
            HostId = hostId,
            SetlistId = createRoomModel.SetlistModel?.Id,
            IsPublic = createRoomModel.IsPublic,
            UpdatedBy = hostId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            Code = code.ToString()
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

    public async Task<RoomModel?> JoinRoomAsync(JoinRoomModel joinRoomModel, User user)
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
            return null;

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

        var totalCount = await query.CountAsync();

        var rooms = await query
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
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .AsNoTracking()
            .ToListAsync();

        PagedResponse<RoomModel> response = new()
        {
            TotalCount = totalCount,
            Items = rooms
                .Select(x => x.ToModel())
                .ToList()
        };

        return response;
    }

    public async Task<RoomModel?> GetRoomByCodeAsync(string roomCode)
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
            return null;
        
        var model = room.ToModel();

        if (room.Setlist is null || model.Setlist is null)
            return model;

        model.Setlist = room.Setlist.ToModel();

        return model;
    }
}