using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface IArtistService
{
    Task<PagedResponse<ArtistModel>> GetAllArtistsAsync(PagedRequest request);
    Task<bool> UploadArtistAsync(ArtistCreateModel createModel);
    Task<ArtistModel> GetArtistByIdAsync(int id);
    Task<bool> TryDeleteArtistAsync(int id);
    Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel);
}