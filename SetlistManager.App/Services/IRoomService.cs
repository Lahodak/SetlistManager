using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface IRoomService
{
    event Action<RoomModel>? RoomUpdated;
    Task<RoomModel?> JoinRoomAsync(JoinRoomModel joinRoomModel);
    Task<RoomModel?> CreateRoomAsync(CreateRoomModel createRoomModel);
    Task<List<RoomModel>?> GetPublicActiveRoomsAsync();
    Task<RoomModel?> ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel);
}