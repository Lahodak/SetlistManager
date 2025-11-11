using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface ISongService
{
    Task<List<SongModel>?> GetAllSongsAsync();
    Task<SongModel?> GetSongByIdAsync(int id);
    Task<SongModel?> GetSongByNameAsync(string name);
    Task UploadSongsAsync(List<SongModel> songsToUpload);
    Task UploadSongAsync(SongCreateModel songCreateModel);
}