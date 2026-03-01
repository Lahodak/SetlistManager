using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

/// <summary>
/// Provides operations for creating, joining, and managing live session rooms.
/// </summary>
public interface IRoomsService
{
    /// <summary>
    /// Creates a new room with an optional setlist.
    /// </summary>
    /// <param name="createRoomModel">The room creation details.</param>
    Task<RoomModel> CreateRoomAsync(RoomCreateModel createRoomModel);

    /// <summary>
    /// Adds a user to an existing room by code.
    /// </summary>
    /// <param name="joinRoomModel">The join request containing the room code.</param>
    /// <param name="user">The user entity joining the room.</param>
    Task<RoomModel> JoinRoomAsync(JoinRoomModel joinRoomModel, User user);

    /// <summary>
    /// Changes the currently active song in a room.
    /// </summary>
    /// <param name="changeCurrentSongModel">The song-change request details.</param>
    Task ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel);

    /// <summary>
    /// Gets a room by its identifier.
    /// </summary>
    /// <param name="roomId">The room identifier.</param>
    Task<RoomModel> GetRoomByIdAsync(int roomId);

    /// <summary>
    /// Gets a paginated list of public, active rooms.
    /// </summary>
    /// <param name="request">Pagination and search parameters.</param>
    Task<PagedResponse<RoomModel>> GetPublicActiveRoomsAsync(PagedRequest request);

    /// <summary>
    /// Gets a room by its unique join code.
    /// </summary>
    /// <param name="roomCode">The room code.</param>
    Task<RoomModel> GetRoomByCodeAsync(string roomCode);
}