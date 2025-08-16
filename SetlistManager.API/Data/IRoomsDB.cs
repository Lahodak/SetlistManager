using SetlistManager.Common.Models;

namespace SetlistManager.API.Data;

public interface IRoomsDB
{
    Task<int> CreateRoomAsync(RoomModel room);
    Task<RoomModel> JoinRoomAsync(string code, UserModel user);
    Task<int> ChangeCurrentSongAsync(int roomId);
}