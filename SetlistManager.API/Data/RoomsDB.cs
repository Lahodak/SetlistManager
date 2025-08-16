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
    public async Task<int> CreateRoomAsync(RoomModel room)
    {
        await _dbContext.AddAsync(new Room().ToEntity(room));
        await _dbContext.SaveChangesAsync();
        int id = (await _dbContext.Rooms.FirstOrDefaultAsync(x => x.Code == room.Code))!.Id;
        return id;
    }

    public async Task<RoomModel> JoinRoomAsync(string code, UserModel user)
    {
        var room = await _dbContext.Rooms
            .FirstOrDefaultAsync(x => x.Code == code) 
            ?? throw new Exception($"Room with code {code} does not exist");

        room.Users.Add(await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id));

        return room.ToModel();
    }

    public async Task<int> ChangeCurrentSongAsync(int roomId)
    {
        var room = await _dbContext.Rooms.FirstOrDefaultAsync(x => x.Id == roomId) ?? throw new Exception($"Room with Id {roomId} does not exist"); ;
        //if(room.RoomsSetlists)
        return 0;
    }
}