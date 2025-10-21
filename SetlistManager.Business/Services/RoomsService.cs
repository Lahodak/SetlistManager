using Microsoft.EntityFrameworkCore;
using SetlistManager.Business.Mappers;
using SetlistManager.Common.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;
using System.Text;

namespace SetlistManager.Business.Services;

public class RoomsService : IRoomsService
{
    private const string roomCodeAvailableCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int roomCodeLength = 6;
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

        
        Random random = new();
        
        StringBuilder code = new(roomCodeLength);
        
        for (int i = 0; i < roomCodeLength; i++)
        {
            int index = random.Next(roomCodeAvailableCharacters.Length - 1);
            code.Append(roomCodeAvailableCharacters[index]);
        }

        room.Code = code.ToString();

        await _dbContext.Rooms.AddAsync(room);

        if (room.Setlist is not null)
            room.CurrentSongId = room.Setlist.SongsSetlists
                .First(x => x.Order == 1).SongId;

        await _dbContext.SaveChangesAsync();

        return room.ToModel();
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

    public async Task<List<RoomModel>> GetPublicRoomsAsync()
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
        
        return room.ToModel();
    }
}