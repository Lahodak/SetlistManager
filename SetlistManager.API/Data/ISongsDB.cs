using SetlistManager.API.Entities;

namespace SetlistManager.API.Data;
public interface ISongsDB
{
    Task<Song?> GetSongByIdAsync(int SongId);
    Task<IEnumerable<Song>> GetSongsAsync();
    Task UploadSongs(Song song);
}