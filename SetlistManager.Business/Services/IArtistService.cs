using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface IArtistService
{
    Task<List<ArtistModel>> GetAllArtistsAsync();
    Task<bool> UploadArtistAsync(ArtistCreateModel createModel);
    Task<ArtistModel> GetArtistByIdAsync(int id);
    Task<bool> TryDeleteArtistAsync(int id);
    Task<bool> TryUpdateArtistAsync(int id, ArtistUpdateModel updateModel);
}