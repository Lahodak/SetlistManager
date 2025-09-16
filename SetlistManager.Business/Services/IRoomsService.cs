using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

public interface IRoomsService
{
    Task CreateRoomAsync(RoomModel room);
    Task<RoomModel> JoinRoomAsync(JoinRoomModel joinRoomModel, User user);
    Task ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel);
    Task<RoomModel?> GetRoomByIdAsync(int roomId); 
}