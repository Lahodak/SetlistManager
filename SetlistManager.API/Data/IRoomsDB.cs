using SetlistManager.Common.Models;

namespace SetlistManager.API.Data;

public interface IRoomsDB
{
    Task<int> CreateRoomAsync (JammingRoomModel room);
    Task<JammingRoomModel> JoinRoomAsync (int id);
}