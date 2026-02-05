using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface ISongService
{
    Task<PagedResponse<SongModel>> GetAllSongsAsync(PagedRequest request);
    Task<SongModel?> GetSongByIdAsync(int id);
    Task UploadSongAsync(SongCreateModel songCreateModel);
    Task<bool> TryUpdateSongAsync(int id, SongUpdateModel songModel); 
    Task<bool> TryDeleteSongAsync(int id);
    Task<bool> TryMakeSongPublicAsync(int id);
    Task<bool> TryGiveAccessToUserAsync(int songId, int targetId);
    Task RemoveAccessFromUserAsync(int songId, int targetId);
    Task<List<SongUsageStatModel>> GetStatisticsAsync(StatsRequest request);
}