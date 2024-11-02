using System.Net.Http.Json;
using SetlistManager.Common.Models;

namespace SetlistManager.Services;

public class LyricsService(IHttpClientFactory httpClientFactory)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    private const string _lyricsApiUrl = "https://api.lyrics.ovh/v1/{0}/{1}";

    public async Task<SongLyrics?> SearchLyricsAsync(SongModel song)
    {
        if (song.Language != Language.EN)
        {
            return null;
        }

        string url = string.Format(_lyricsApiUrl, song.Artist, song.Name);

        using var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var songLyrics = await response.Content.ReadFromJsonAsync<SongLyrics>();
        return songLyrics;
    }
}

public class SongLyrics
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Lyrics { get; set; }
}