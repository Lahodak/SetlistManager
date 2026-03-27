using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

/// <summary>
/// Provides CRUD and access-control operations for setlists.
/// </summary>
public interface ISetlistsService
{
    /// <summary>
    /// Gets a paginated list of setlists accessible to the current user.
    /// </summary>
    /// <param name="request">Pagination and search parameters.</param>
    Task<PagedResponse<SetlistModel>> GetSetlistsAsync(PagedRequest request);

    /// <summary>
    /// Gets a setlist by its identifier.
    /// </summary>
    /// <param name="setlistId">The setlist identifier.</param>
    Task<SetlistModel> GetSetlistByIdAsync(int setlistId);

    /// <summary>
    /// Updates an existing setlist, including its song ordering.
    /// </summary>
    /// <param name="setlistModel">The setlist data to apply.</param>
    Task EditSetlistAsync(SetlistModel setlistModel);

    /// <summary>
    /// Creates a new setlist with the specified songs and ordering.
    /// </summary>
    /// <param name="createModel">The setlist creation details.</param>
    Task CreateSetlistAsync(SetlistCreateModel createModel);

    /// <summary>
    /// Deletes a setlist by its identifier.
    /// </summary>
    /// <param name="setlistId">The setlist identifier.</param>
    Task DeleteSetlistAsync(int setlistId);

    /// <summary>
    /// Grants a user access to a setlist.
    /// </summary>
    /// <param name="setlistId">The setlist identifier.</param>
    /// <param name="targetId">The user identifier to grant access to.</param>
    Task GiveAccessToSetlistAsync(int setlistId, int targetId);

    /// <summary>
    /// Revokes a user's access to a setlist.
    /// </summary>
    /// <param name="setlistId">The setlist identifier.</param>
    /// <param name="userId">The user identifier to revoke access from.</param>
    Task RemoveAccessFromUserAsync(int setlistId, int userId);
}