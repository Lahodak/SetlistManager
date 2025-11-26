using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;
public interface ISongService
{
    Task<SongModel?> GetSongByIdAsync(int songId);
    Task<List<SongModel>?> GetSongsAsync();
    Task<bool> TrySaveSongAsync(SongCreateModel songCreateModel, int userId);
    Task<bool> TryUpdateSongAsync(int songId, SongUpdateModel updateModel, int userId);
    Task<bool> TryDeleteSongAsync(int songId);
}