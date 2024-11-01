using SetlistManager.API.Models;

namespace SetlistManager.API.Data;
public interface ISongsDB
{
    Task<Song?> GetSongByIdAsync(int SongId);
    Task<IEnumerable<Song>> GetSongsAsync();
}