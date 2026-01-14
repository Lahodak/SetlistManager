using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface ISongService
{
    Task<SongModel?> GetPublicSongByIdAsync(int id);
    Task<PagedResponse<SongModel>> GetPublicSongsAsync(PagedRequest request);
    Task<bool> TryCreateSongAsync(SongCreateModel songCreateModel, int userId);
    Task<bool> TryUpdateSongAsync(int songId, SongUpdateModel updateModel, int userId);
    Task<bool> TryDeleteSongAsync(int songId);
    Task<PagedResponse<SongModel>> GetSongLibraryByUserId(int userId, PagedRequest request);
    Task<bool> TryGiveAccessToUserAsync(int songId, int targetId);
    Task<bool> TryMakeSongPublicAsync(int songId);
    Task<SongModel?> GetUserSongById(int userId, int songId);
}