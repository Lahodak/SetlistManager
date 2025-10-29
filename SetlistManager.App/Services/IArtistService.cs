using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface IArtistService
{
    Task<List<ArtistModel>?> GetAvailableArtistsAsync();
    Task<ArtistModel?> GetArtistByIdAsync(int id);
    Task UploadArtistAsync(ArtistModel artist);
}