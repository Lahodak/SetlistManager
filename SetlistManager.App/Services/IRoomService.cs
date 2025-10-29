using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface IRoomService
{
    Task JoinRoomAsync(JoinRoomModel joinRoomModel);
    Task<RoomModel?> CreateRoomAsync(CreateRoomModel createRoomModel);
    Task<RoomModel?> GetRoomByCodeAsync(string roomCode);
    Task<List<RoomModel>?> GetPublicActiveRoomsAsync();
}