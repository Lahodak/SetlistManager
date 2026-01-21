using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface IArtistService
{
    Task<PagedResponse<ArtistModel>> GetArtistsAsync(PagedRequest request, int userId);
    Task<ArtistModel?> GetArtistByIdAsync(int artistId, int userId, ContentType contentType);
    Task<bool> TryDeleteArtistAsync(int artistId, int userId);
    Task<bool> TryCreateArtistAsync(ArtistCreateModel createModel, int creatorId);
    Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel, int currentUserId);
    Task<bool> TryMakeArtistPublicAsync(int artistId, int currentUserId);
    Task<bool> TryGiveAccessToUserAsync(int artistId, int targetId, int currentUserId);
    Task RemoveAccessFromUserAsync(int artistId, int targetId, int currentUserId);
}