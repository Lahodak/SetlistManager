using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

/// <summary>
/// Provides CRUD and access-control operations for artists.
/// </summary>
public interface IArtistService
{
    /// <summary>
    /// Gets a paginated list of artists filtered by content type.
    /// </summary>
    /// <param name="request">Pagination, search, and content-type parameters.</param>
    Task<PagedResponse<ArtistModel>> GetArtistsAsync(ContentPagedRequest request);

    /// <summary>
    /// Gets an artist by its identifier, scoped to the specified content type.
    /// </summary>
    /// <param name="artistId">The artist identifier.</param>
    /// <param name="contentType">The visibility scope to apply.</param>
    /// <returns>The artist model, or <see langword="null"/> if not found.</returns>
    Task<ArtistModel?> GetArtistByIdAsync(int artistId, ContentType contentType);

    /// <summary>
    /// Deletes an artist by its identifier.
    /// </summary>
    /// <param name="artistId">The artist identifier.</param>
    Task DeleteArtistAsync(int artistId);

    /// <summary>
    /// Creates a new artist.
    /// </summary>
    /// <param name="createModel">The artist creation details.</param>
    Task CreateArtistAsync(ArtistCreateModel createModel);

    /// <summary>
    /// Updates an existing artist.
    /// </summary>
    /// <param name="id">The artist identifier.</param>
    /// <param name="updateModel">The updated artist data.</param>
    Task UpdateArtistAsync(int id, ArtistUpdateModel updateModel);

    /// <summary>
    /// Makes an artist publicly visible to all users.
    /// </summary>
    /// <param name="artistId">The artist identifier.</param>
    Task MakeArtistPublicAsync(int artistId);

    /// <summary>
    /// Grants a user access to an artist.
    /// </summary>
    /// <param name="artistId">The artist identifier.</param>
    /// <param name="targetId">The user identifier to grant access to.</param>
    Task GiveAccessToUserAsync(int artistId, int targetId);

    /// <summary>
    /// Revokes a user's access to an artist.
    /// </summary>
    /// <param name="artistId">The artist identifier.</param>
    /// <param name="targetId">The user identifier to revoke access from.</param>
    Task RemoveAccessFromUserAsync(int artistId, int targetId);
}