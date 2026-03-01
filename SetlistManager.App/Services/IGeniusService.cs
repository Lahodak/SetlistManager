using SetlistManager.App.Models;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

/// <summary>
/// Client-side service for Genius lyrics integration.
/// </summary>
public interface IGeniusService
{
    /// <summary>Fetches lyrics embed data for the specified song from the Genius API.</summary>
    /// <param name="song">The song to look up lyrics for.</param>
    /// <returns>The embed model, or <see langword="null"/> if no match was found.</returns>
    Task<GeniusEmbedModel?> FetchSongLyricsAsync(SongModel song);

    /// <summary>Initiates the Genius OAuth authorization flow and returns the redirect URL.</summary>
    Task<string> AuthorizeAsync();
}