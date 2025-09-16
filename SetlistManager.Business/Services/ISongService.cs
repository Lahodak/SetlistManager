using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services;
public interface ISongService
{
    Task<Song?> GetSongByIdAsync(int SongId);
    Task<IEnumerable<Song>> GetSongsAsync();
    Task UploadSongAsync(Song Song);
    Task<IEnumerable<Song?>> GetSongByNameAsync(string Name);
}