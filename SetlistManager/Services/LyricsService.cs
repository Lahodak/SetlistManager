using Newtonsoft.Json;
using SetlistManager.Common.Models;
using SetlistManager.Models;

namespace SetlistManager.Services;

public class LyricsService(IHttpClientFactory httpClientFactory)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    private const string _lyricsApiUrl = "https://api.lyrics.ovh/v1/{0}/{1}";

    public async Task<SongLyrics?> SearchLyricsAsync(SongModel song)
    {
        if (song.Language.Code != "EN")
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

        var songLyricsJson = await response.Content.ReadAsStringAsync();
        
        try
        {
            var songLyrics = JsonConvert.DeserializeObject<SongLyrics>(songLyricsJson);
            return songLyrics;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);  
            return null;
        }
    }
}