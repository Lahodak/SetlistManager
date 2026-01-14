using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface IArtistService
{
    Task<PagedResponse<ArtistModel>> GetPublicArtistsAsync(PagedRequest request);
    Task<ArtistModel?> GetPublicArtistByIdAsync(int id);
    Task<bool> TryDeleteArtistAsync(int artistId, int userId);
    Task<bool> TryCreateArtistAsync(ArtistCreateModel createModel, int creatorId);
    Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel);
    Task<PagedResponse<ArtistModel>> GetUserArtistLibraryAsync(PagedRequest request, int userId);
    Task<ArtistModel?> GetUserArtistById(int artistId, int userId);
    Task<bool> TryMakeArtistPublicAsync(int artistId);
    Task<bool> TryGiveAccessToUserAsync(int artistId, int targetId);
}