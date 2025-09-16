using SetlistManager.Common.Models;
using Microsoft.EntityFrameworkCore;
using SetlistManager.Data.Entities;
using SetlistManger.Business.Mappers;
using SetlistManager.Data;

namespace SetlistManger.Business.Services;

public class RoomsService : IRoomsService
{
    private readonly AppDbContext _dbContext;

    public RoomsService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<RoomModel?> GetRoomByIdAsync(int roomId)
    {
        var room = await _dbContext.Rooms
            .Include(x => x.Setlist)
            .ThenInclude(x => x!.SongsSetlists)
            .ThenInclude(x => x.Song)
            .FirstOrDefaultAsync(x => x.Id == roomId);

        if (room is null)
            return null;

        return room.ToModel();
    }

    public async Task CreateRoomAsync(RoomModel room)
    {
        await _dbContext.AddAsync(room.ToEntity());        
        await _dbContext.SaveChangesAsync();                
    }

    public async Task<RoomModel> JoinRoomAsync(JoinRoomModel joinRoomModel, User user)
    {
        var room = await _dbContext.Rooms
            .Include(x => x.Users)
            .ThenInclude(x => x.Instrument)
            .Include(x => x.Setlist)            
            .FirstOrDefaultAsync(x => x.Code == joinRoomModel.RoomCode)
            ?? throw new Exception($"Room with code {joinRoomModel.RoomCode} does not exist");

        room.Users.Add(user);

        return room.ToModel();
    }

    public async Task ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel)
    {
        var room = await _dbContext.Rooms
            .Include(x => x.Setlist)
            .ThenInclude(y => y!.SongsSetlists)
            .ThenInclude(z => z.Song)
            .FirstOrDefaultAsync(x => x.Id == changeCurrentSongModel.RoomId)
            ?? throw new Exception($"Room with id {changeCurrentSongModel.RoomId} does not exist");

        room.CurrentSongId = changeCurrentSongModel.NewCurrentSongId;

        _dbContext.Rooms.Update(room);
        _dbContext.SaveChanges();

        return;
    }
}