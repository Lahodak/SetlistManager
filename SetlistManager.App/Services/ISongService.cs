using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface ISongService
{
    Task<PagedResponse<SongModel>> GetSongsAsync(ContentPagedRequest request);
    Task<SongModel?> GetSongByIdAsync(int id);
    Task<bool> TryCreateSongAsync(SongCreateModel songCreateModel);
    Task<bool> TryUpdateSongAsync(int id, SongUpdateModel songModel); 
    Task<bool> TryDeleteSongAsync(int id);
    Task<bool> TryGiveAccessToUserAsync(int songId, int targetId);
    Task<bool> TryRemoveAccessFromUserAsync(int songId, int targetId);
    Task<bool> TryMakeSongPublicAsync(int id);
    Task<List<SongUsageStatModel>> GetStatisticsAsync(StatsRequest request);
}