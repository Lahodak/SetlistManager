using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

/// <summary>
/// Client-side service for managing setlists via the API.
/// </summary>
public interface ISetlistService
{
    /// <summary>Gets a paginated list of setlists.</summary>
    Task<PagedResponse<SetlistModel>> GetSetlistsAsync(PagedRequest request);

    /// <summary>Gets a setlist by its identifier.</summary>
    Task<SetlistModel?> GetSetlistById(int id);

    /// <summary>Creates a new setlist. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryCreateSetlistAsync(SetlistCreateModel createModel);

    /// <summary>Updates an existing setlist. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryEditSetlist(SetlistModel setlistModel);

    /// <summary>Deletes a setlist. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryDeleteSetlistAsync(int id);

    /// <summary>Grants a user access to a setlist. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryGiveAccessToUserAsync(int setlistId, int targetId);

    /// <summary>Revokes a user's access to a setlist. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryRemoveAccessFromUserAsync(int setlistId, int targetId);
}