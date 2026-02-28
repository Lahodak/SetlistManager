using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface IArtistService
{
    Task<PagedResponse<ArtistModel>> GetArtistsAsync(ContentPagedRequest request);
    Task<ArtistModel?> GetArtistByIdAsync(int artistId, ContentType contentType);
    Task DeleteArtistAsync(int artistId);
    Task CreateArtistAsync(ArtistCreateModel createModel);
    Task UpdateArtistAsync(int id, ArtistUpdateModel updateModel);
    Task MakeArtistPublicAsync(int artistId);
    Task GiveAccessToUserAsync(int artistId, int targetId);
    Task RemoveAccessFromUserAsync(int artistId, int targetId);
}