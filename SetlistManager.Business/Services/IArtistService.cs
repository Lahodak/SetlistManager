using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface IArtistService
{
    Task<PagedResponse<ArtistModel>> GetArtistsAsync(PagedRequest request);
    Task<ArtistModel?> GetArtistByIdAsync(int artistId, ContentType contentType);
    Task<bool> TryDeleteArtistAsync(int artistId);
    Task TryCreateArtistAsync(ArtistCreateModel createModel);
    Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel);
    Task<bool> TryMakeArtistPublicAsync(int artistId);
    Task<bool> TryGiveAccessToUserAsync(int artistId, int targetId);
    Task RemoveAccessFromUserAsync(int artistId, int targetId);
}