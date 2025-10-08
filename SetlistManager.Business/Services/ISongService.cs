using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;
public interface ISongService
{
    Task<Song?> GetSongByIdAsync(int songId);
    Task<IEnumerable<Song>> GetSongsAsync();
    Task UploadSongAsync(Song song);
    Task<IEnumerable<Song?>> GetSongByNameAsync(string name);
}