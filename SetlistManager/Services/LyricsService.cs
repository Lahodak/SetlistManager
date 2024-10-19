using System.Net.Http.Json;
using SetlistManager.Models;

namespace SetlistManager.Services;

public static class LyricsService
{
    private static readonly HttpClient _httpClient = new();
    public static async Task<SongLyrics> SearchLyricsAsync(Song song)
    {
        if (song.Language != Language.EN)
            return null;
        string url = $"https://api.lyrics.ovh/v1/{song.Artist}/{song.Name}";

        var response = await _httpClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var songLyrics = await response.Content.ReadFromJsonAsync<SongLyrics>();
            return songLyrics;
        }
        return null;
    }
}
public class SongLyrics
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public string Lyrics { get; set; }
}