using SetlistManager.Common.Models;
using SetlistManager.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace SetlistManager.API.Data;

public class RoomsDB : IRoomsDB
{
    private readonly APIDbContext _dbContext;

    public RoomsDB (APIDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RoomModel> GetRoomById(int roomId)
    {
        Room room = await _dbContext.Rooms.Include(x => x.Setlist)
            .ThenInclude(x => x.SongsSetlists)
            .ThenInclude(x => x.Song)
            .FirstAsync(x => x.Id == roomId) 
            ?? throw new Exception();
        return room.ToModel();
    }

    public async Task<RoomModel> CreateRoomAsync(RoomModel room)
    {
        var x = await _dbContext.AddAsync(new Room().ToEntity(room));
        await _dbContext.SaveChangesAsync();
        var id = (await _dbContext.Rooms.FirstOrDefaultAsync(x => x.Code == room.Code))!.Id;
        return new();
    }

    public async Task<RoomModel> JoinRoomAsync(JoinRoomModel joinRoomModel, User user)
    {
        var room = await _dbContext.Rooms
            .Include(x => x.Users)!
            .ThenInclude(x => x.Instrument)
            .Include(x => x.Setlist)            
            .FirstOrDefaultAsync(x => x.Code == joinRoomModel.RoomCode)
            ?? throw new Exception($"Room with code {joinRoomModel.RoomCode} does not exist");

        room.Users.Add(user);

        return room.ToModel();
    }

    public async Task<int> ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel)
    {
        var room = await _dbContext.Rooms
            .Include(x => x.Setlist)
            .ThenInclude(y => y.SongsSetlists)
            .ThenInclude(z => z.Song)
            .FirstOrDefaultAsync(x => x.Id == changeCurrentSongModel.RoomId) 
                ?? throw new Exception($"Room with Id {changeCurrentSongModel.RoomId} does not exist");

        if (room.Setlist == null || room.Setlist.SongsSetlists == null || !room.Setlist.SongsSetlists.Any())
            throw new Exception("Room does not have a valid setlist with songs");

        return 0;
    }
}