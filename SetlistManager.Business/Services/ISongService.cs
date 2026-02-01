using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface ISongService
{
    Task<SongModel?> GetSongByIdAsync(int songId);
    Task<PagedResponse<SongModel>> GetSongsAsync(PagedRequest request);
    Task<bool> TryCreateSongAsync(SongCreateModel songCreateModel);
    Task<bool> TryUpdateSongAsync(int songId, SongUpdateModel updateModel);
    Task<bool> TryDeleteSongAsync(int songId);
    Task<bool> TryGiveAccessToUserAsync(int songId, int targetId);
    Task RemoveAccessFromUserAsync(int songId, int userId);
    Task<bool> TryMakeSongPublicAsync(int songId);
    Task<PagedResponse<SongUsageStatModel>> GetMostUsedSongsAsync(StatsPagedRequest request);
    Task<PagedResponse<SongUsageStatModel>> GetMostAddedToLibraryAsync(PagedRequest request);
    Task<PagedResponse<LatestSongStatModel>> GetLatestPublicSongsAsync(PagedRequest request);
}