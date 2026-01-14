using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface ISongService
{
    Task<SongModel?> GetSongByIdAsync(int songId, int userId);
    Task<PagedResponse<SongModel>> GetSongsAsync(PagedRequest request, int userId);
    Task<bool> TryCreateSongAsync(SongCreateModel songCreateModel, int userId);
    Task<bool> TryUpdateSongAsync(int songId, SongUpdateModel updateModel, int userId);
    Task<bool> TryDeleteSongAsync(int songId, int userId);
    Task<bool> TryGiveAccessToUserAsync(int songId, int targetId, int currentUserId);
    Task<bool> TryMakeSongPublicAsync(int songId, int userId);
}