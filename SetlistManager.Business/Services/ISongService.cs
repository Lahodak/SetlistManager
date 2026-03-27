using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

/// <summary>
/// Provides CRUD, access-control, and statistics operations for songs.
/// </summary>
public interface ISongService
{
    /// <summary>
    /// Gets a song by its identifier.
    /// </summary>
    /// <param name="songId">The song identifier.</param>
    /// <returns>The song model, or <see langword="null"/> if not found.</returns>
    Task<SongModel?> GetSongByIdAsync(int songId);

    /// <summary>
    /// Gets a paginated list of songs filtered by content type.
    /// </summary>
    /// <param name="request">Pagination, search, and content-type parameters.</param>
    Task<PagedResponse<SongModel>> GetSongsAsync(ContentPagedRequest request);

    /// <summary>
    /// Creates a new song.
    /// </summary>
    /// <param name="songCreateModel">The song creation details.</param>
    Task CreateSongAsync(SongCreateModel songCreateModel);

    /// <summary>
    /// Updates an existing song.
    /// </summary>
    /// <param name="songId">The song identifier.</param>
    /// <param name="updateModel">The updated song data.</param>
    Task UpdateSongAsync(int songId, SongUpdateModel updateModel);

    /// <summary>
    /// Deletes a song by its identifier.
    /// </summary>
    /// <param name="songId">The song identifier.</param>
    Task DeleteSongAsync(int songId);

    /// <summary>
    /// Grants a user access to a song.
    /// </summary>
    /// <param name="songId">The song identifier.</param>
    /// <param name="targetId">The user identifier to grant access to.</param>
    Task GiveAccessToUserAsync(int songId, int targetId);

    /// <summary>
    /// Revokes a user's access to a song.
    /// </summary>
    /// <param name="songId">The song identifier.</param>
    /// <param name="userId">The user identifier to revoke access from.</param>
    Task RemoveAccessFromUserAsync(int songId, int userId);

    /// <summary>
    /// Makes a song publicly visible to all users.
    /// </summary>
    /// <param name="songId">The song identifier.</param>
    Task MakeSongPublicAsync(int songId);

    /// <summary>
    /// Gets song usage statistics based on the specified criteria.
    /// </summary>
    /// <param name="request">The statistics query parameters.</param>
    Task<List<SongUsageStatModel>> GetStatisticsAsync(StatsRequest request);
}