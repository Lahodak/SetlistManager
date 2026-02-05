using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface ISongService
{
    Task<SongModel?> GetSongByIdAsync(int songId);
    Task<PagedResponse<SongModel>> GetSongsAsync(PagedRequest request);
    Task TryCreateSongAsync(SongCreateModel songCreateModel);
    Task TryUpdateSongAsync(int songId, SongUpdateModel updateModel);
    Task TryDeleteSongAsync(int songId);
    Task TryGiveAccessToUserAsync(int songId, int targetId);
    Task RemoveAccessFromUserAsync(int songId, int userId);
    Task TryMakeSongPublicAsync(int songId);
    Task<List<SongUsageStatModel>> GetStatisticsAsync(StatsRequest request);
}