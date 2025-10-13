using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;

public interface IArtistService
{
    Task<List<ArtistModel>> GetAllArtistsAsync();
    Task UploadArtistAsync(ArtistModel artistModel);
    Task<ArtistModel> GetArtistModelByIdAsync(int id);
    Task<Artist> GetArtistByIdAsync(int id);
}