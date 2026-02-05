using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface IArtistService
{
    Task<PagedResponse<ArtistModel>> GetArtistsAsync(PagedRequest request);
    Task<ArtistModel?> GetArtistByIdAsync(int artistId, ContentType contentType);
    Task TryDeleteArtistAsync(int artistId);
    Task TryCreateArtistAsync(ArtistCreateModel createModel);
    Task TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel);
    Task TryMakeArtistPublicAsync(int artistId);
    Task TryGiveAccessToUserAsync(int artistId, int targetId);
    Task RemoveAccessFromUserAsync(int artistId, int targetId);
}