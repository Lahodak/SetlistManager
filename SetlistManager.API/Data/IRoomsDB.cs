using SetlistManager.Common.Models;

namespace SetlistManager.API.Data;

public interface IRoomsDB
{
    Task<int> CreateRoomAsync(RoomModel room);
    Task<RoomModel> JoinRoomAsync(int id, UserModel user);
    Task<int> ChangeCurrentSongAsync(int roomId);
}