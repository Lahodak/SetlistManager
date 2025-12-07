using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface IArtistService
{
    Task<PagedResponse<ArtistModel>?> GetAvailableArtistsAsync(PagedRequest request);
    Task<ArtistModel?> GetArtistByIdAsync(int id);
    Task UploadArtistAsync(ArtistCreateModel createModel);
    Task<bool> TryDeleteArtistAsync(int id);
    Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel);
}