using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface IRoomService
{
    event Action<RoomModel>? RoomUpdated;
    Task<RoomModel?> JoinRoomAsync(JoinRoomModel joinRoomModel);
    Task<RoomModel?> CreateRoomAsync(RoomCreateModel createModel);
    Task<PagedResponse<RoomModel>> GetPublicActiveRoomsAsync(PagedRequest request);
    Task<RoomModel?> ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel);
}