using SetlistManager.Common.Models;
using SetlistManager.API.Data.Entities;

namespace SetlistManager.API.Data;

public interface IRoomsDB
{
    Task<int> CreateRoomAsync(RoomModel room);
    Task<RoomModel> JoinRoomAsync(JoinRoomModel joinRoomModel, User user);
    Task<int> ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel);
    Task<RoomModel> GetRoomById(int roomId); 
}