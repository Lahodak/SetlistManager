using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface IRoomService
{
    Task<RoomModel?> JoinRoomAsync(JoinRoomModel joinRoomModel);
    Task<RoomModel?> CreateRoomAsync(CreateRoomModel createRoomModel);
    Task<List<RoomModel>?> GetPublicActiveRoomsAsync();
}