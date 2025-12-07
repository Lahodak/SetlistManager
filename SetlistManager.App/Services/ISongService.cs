using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface ISongService
{
    Task<PagedResponse<SongModel>?> GetAllSongsAsync(PagedRequest request);
    Task<SongModel?> GetSongByIdAsync(int id);
    Task UploadSongAsync(SongCreateModel songCreateModel);
    Task<bool> TryUpdateSongAsync(int id, SongUpdateModel songModel); 
    Task<bool> TryDeleteSongAsync(int id);
}