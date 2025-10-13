using SetlistManager.Common.Models;
using Microsoft.EntityFrameworkCore;
using SetlistManager.Data.Entities;
using SetlistManager.Business.Mappers;
using SetlistManager.Data;

namespace SetlistManager.Business.Services;

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

    public async Task<RoomModel?> JoinRoomAsync(JoinRoomModel joinRoomModel, User user)
    {
        var room = await _dbContext.Rooms
            .Include(x => x.Users)
            .ThenInclude(x => x.Instrument)
            .Include(x => x.Setlist)
            .FirstOrDefaultAsync(x => x.Code == joinRoomModel.RoomCode);

        if(room is null)
            return null;

        room.Users.Add(user);

        await _dbContext.SaveChangesAsync();
        return room.ToModel();
    }

    public async Task ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel)
    {
        var room = await _dbContext.Rooms
            .Include(x => x.Setlist)
            .ThenInclude(y => y!.SongsSetlists)
            .ThenInclude(z => z.Song)
            .FirstOrDefaultAsync(x => x.Id == changeCurrentSongModel.RoomId);

        if(room is null) 
            return;    

        room.CurrentSongId = changeCurrentSongModel.NewCurrentSongId;

        _dbContext.Rooms.Update(room);
        await _dbContext.SaveChangesAsync();
    }
}