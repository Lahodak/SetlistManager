using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface IGeniusService
{
    Task<string?> FetchSongLyricsAsync(SongModel song);
}