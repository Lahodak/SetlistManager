using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

/// <summary>
/// Client-side service for managing artists via the API.
/// </summary>
public interface IArtistService
{
    /// <summary>Gets a paginated list of artists filtered by content type.</summary>
    Task<PagedResponse<ArtistModel>> GetArtistsAsync(ContentPagedRequest request);

    /// <summary>Gets an artist by its identifier.</summary>
    Task<ArtistModel?> GetArtistByIdAsync(int id);

    /// <summary>Creates a new artist. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryCreateArtistAsync(ArtistCreateModel createModel);

    /// <summary>Updates an existing artist. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel);

    /// <summary>Deletes an artist. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryDeleteArtistAsync(int id);

    /// <summary>Grants a user access to an artist. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryGiveAccessToUserAsync(int artistId, int targetId);

    /// <summary>Revokes a user's access to an artist. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryRemoveAccessFromUserAsync(int artistId, int targetId);

    /// <summary>Makes an artist publicly visible. Returns <see langword="true"/> on success.</summary>
    Task<bool> TryMakeArtistPublicAsync(int id);
}