using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;
public interface ISongService
{
    Task<SongModel?> GetSongByIdAsync(int songId);
    Task<List<SongModel>?> GetSongsAsync();
    Task UploadSongAsync(SongCreateModel songCreateModel, int userId);
    Task<List<SongModel>?> GetSongByNameAsync(string name);
}