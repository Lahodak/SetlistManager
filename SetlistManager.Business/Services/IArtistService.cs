using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface IArtistService
{
    Task<List<ArtistModel>> GetAllArtistsAsync();
    Task UploadArtistAsync(ArtistModel artistModel);
    Task<ArtistModel> GetArtistByIdAsync(int id);
}