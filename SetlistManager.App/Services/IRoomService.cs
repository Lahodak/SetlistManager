using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

/// <summary>
/// Client-side service for managing live session rooms, including real-time updates.
/// </summary>
public interface IRoomService
{
    /// <summary>Raised when the room state is updated (e.g., via SignalR).</summary>
    event Action<RoomModel>? RoomUpdated;

    /// <summary>Joins an existing room by code and begins receiving real-time updates.</summary>
    Task<RoomModel?> JoinRoomAsync(JoinRoomModel joinRoomModel);

    /// <summary>Creates a new room with an optional setlist.</summary>
    Task<RoomModel?> CreateRoomAsync(RoomCreateModel createModel);

    /// <summary>Gets a paginated list of public, active rooms.</summary>
    Task<PagedResponse<RoomModel>> GetPublicActiveRoomsAsync(PagedRequest request);

    /// <summary>Changes the currently active song in a room.</summary>
    Task<RoomModel?> ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel);
}