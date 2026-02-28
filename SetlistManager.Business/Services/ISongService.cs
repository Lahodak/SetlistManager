using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface ISongService
{
    Task<SongModel?> GetSongByIdAsync(int songId);
    Task<PagedResponse<SongModel>> GetSongsAsync(ContentPagedRequest request);
    Task CreateSongAsync(SongCreateModel songCreateModel);
    Task UpdateSongAsync(int songId, SongUpdateModel updateModel);
    Task DeleteSongAsync(int songId);
    Task GiveAccessToUserAsync(int songId, int targetId);
    Task RemoveAccessFromUserAsync(int songId, int userId);
    Task MakeSongPublicAsync(int songId);
    Task<List<SongUsageStatModel>> GetStatisticsAsync(StatsRequest request);
}