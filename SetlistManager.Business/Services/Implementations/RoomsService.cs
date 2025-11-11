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
    private readonly IOrderMappingService _orderMappingService;

    public RoomsService(AppDbContext dbContext, IOrderMappingService orderMappingService)
    {
        _dbContext = dbContext;
        _orderMappingService = orderMappingService;
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

        if(room is null)
            return null;

        var roomModel = room.ToModel();

        if (room.Setlist is null)
            return roomModel;

        roomModel.Setlist = await _orderMappingService.MapSongEntityToModelOrder(room.Setlist);

        return roomModel;
    }

    public async Task<RoomModel> CreateRoomAsync(CreateRoomModel createRoomModel, int hostId)
    {
        Room room = new()
        {
            Name = createRoomModel.Name,
            HostId = hostId,
            SetlistId = createRoomModel.SetlistModel!.Id,
            IsPublic = createRoomModel.IsPublic,
            UpdatedBy = hostId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
                
        StringBuilder code = new(roomCodeLength);
        
        for (int i = 0; i < roomCodeLength; i++)
        {
            int index = Random.Shared.Next(roomCodeAvailableCharacters.Length - 1);
            code.Append(roomCodeAvailableCharacters[index]);
        }

        room.Code = code.ToString();

        await _dbContext.Rooms.AddAsync(room);
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

        if (createdRoom.Setlist is null)
            return roomModel;

        roomModel.Setlist = await _orderMappingService.MapSongEntityToModelOrder(createdRoom.Setlist);

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

        var roomModel = room.ToModel();

        if (room.Setlist is null)
            return roomModel;

        roomModel.Setlist = await _orderMappingService.MapSongEntityToModelOrder(room.Setlist);

        await _dbContext.SaveChangesAsync();
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

    public async Task<List<RoomModel>> GetPublicActiveRoomsAsync()
    {
        var rooms = await _dbContext.Rooms            
            .Where(x => x.IsPublic)
            .Where(x => x.IsActive)
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
            .ToListAsync();

        return rooms.Select(x => x.ToModel()).ToList();
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

        if (room.Setlist is null)
            return model;

        model.Setlist = await _orderMappingService.MapSongEntityToModelOrder(room.Setlist);

        return model;
    }
}