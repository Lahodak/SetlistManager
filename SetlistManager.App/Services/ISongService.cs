using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

/// <summary>
/// Client-side service for managing songs via the API.
/// </summary>
public interface ISongService
{
    /// <summary>Gets a paginated list of songs filtered by content type.</summary>
    Task<PagedResponse<SongModel>> GetSongsAsync(ContentPagedRequest request);

    /// <summary>Gets a song by its identifier.</summary>
    Task<SongModel?> GetSongByIdAsync(int id);

    /// <summary>Creates a new song. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryCreateSongAsync(SongCreateModel songCreateModel);

    /// <summary>Updates an existing song. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryUpdateSongAsync(int id, SongUpdateModel songModel);

    /// <summary>Deletes a song. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryDeleteSongAsync(int id);

    /// <summary>Grants a user access to a song. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryGiveAccessToUserAsync(int songId, int targetId);

    /// <summary>Revokes a user's access to a song. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryRemoveAccessFromUserAsync(int songId, int targetId);

    /// <summary>Makes a song publicly visible. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryMakeSongPublicAsync(int id);

    /// <summary>Gets song usage statistics based on the specified criteria.</summary>
    Task<List<SongUsageStatModel>> GetStatisticsAsync(StatsRequest request);
}