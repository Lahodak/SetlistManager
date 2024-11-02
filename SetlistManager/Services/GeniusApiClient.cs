using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SetlistManager.Models;
namespace SetlistManager.Services;

public class GeniusApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _accessToken = "baHzM8Sb4mdTPjZKMNE_Ac3r818gj2n2ygt9fKtj2ETIhueTXuQqcmy56IVbJEGL";

    public GeniusApiClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    public async Task<string> GetLyrics(string artistName, string songTitle)
    {
        var searchUrl = $"https://api.genius.com/search?q={Uri.EscapeDataString(artistName + " " + songTitle)}";
        var searchResponse = await _httpClient.GetStringAsync(searchUrl);
        var searchResult = JObject.Parse(searchResponse);

        var hit = searchResult["response"]["hits"]?.FirstOrDefault();
        if (hit != null)
        {
            var songPath = hit["result"]["path"].ToString();
            var songUrl = $"https://genius.com{songPath}";
            var lyrics = await GetLyricsFromUrl(songUrl);
            return lyrics;
        }

        return "Lyrics not found";
    }

    private async Task<string> GetLyricsFromUrl(string songUrl)
    {
        var lyricsResponse = await _httpClient.GetStringAsync(songUrl);
        var doc = new HtmlAgilityPack.HtmlDocument();
        doc.LoadHtml(lyricsResponse);

        var lyricsNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'lyrics')]");

        return lyricsNode?.InnerText.Trim() ?? "Lyrics not found";
    }
    public async Task FetchAndDisplayLyrics(Song song)
    {
        var geniusApiClient = new GeniusApiClient();

        string lyrics = await geniusApiClient.GetLyrics(song.Artist, song.Name);
    }
}