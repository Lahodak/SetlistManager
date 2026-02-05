using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface IArtistService
{
    Task<PagedResponse<ArtistModel>> GetArtistsAsync(PagedRequest request);
    Task<ArtistModel?> GetArtistByIdAsync(int id);
    Task<bool> TryCreateArtistAsync(ArtistCreateModel createModel);
    Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel);
    Task<bool> TryDeleteArtistAsync(int id);
    Task<bool> TryGiveAccessToUserAsync(int artistId, int targetId);
    Task<bool> TryRemoveAccessFromUserAsync(int artistId, int targetId);
    Task<bool> TryMakeArtistPublicAsync(int id);
}