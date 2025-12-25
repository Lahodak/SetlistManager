using SetlistManager.App.Models;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public interface IGeniusService
{
    Task<GeniusEmbedModel?> FetchSongLyricsAsync(SongModel song);
    Task<string> AuthorizeAsync();
}