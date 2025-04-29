using SetlistManager.API.Entities;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Data;
public interface ISongsDB
{
    Task<SongModel?> GetSongByIdAsync(int SongId);
    Task<IEnumerable<SongModel>> GetSongsAsync();
    Task UploadSongs(SongModel Song);
}