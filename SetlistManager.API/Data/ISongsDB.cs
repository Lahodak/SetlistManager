using SetlistManager.API;
using SetlistManager.API.Data.Entities;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Data;
public interface ISongsDB
{
    Task<Song?> GetSongByIdAsync(int SongId);
    Task<IEnumerable<Song>> GetSongsAsync();
    Task UploadSong(Song Song);
}