using Microsoft.AspNetCore.Http.Extensions;
using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;
using System.Net.Http.Json;
using Newtonsoft.Json;
using SetlistManager.Common.Genius.Models.Search;
using SetlistManager.Common.Genius.Models.Songs;
using MudBlazor.Interfaces;

namespace SetlistManager.App.Services;

public class GeniusService
{
    private const string _baseApiUrl = "https://api.genius.com";
    private const string _searchEndpointSuffix = "/search?";
    private const string _textFormat = "html";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UserService _userService;
    public GeniusService(IHttpClientFactory factory, IConfiguration configuration, UserService userService)
    {
        _httpClientFactory = factory;
        _userService = userService;
    }

    public async Task<string> FetchSongLyricsAsync(SongModel song)
    {
        var client = _httpClientFactory.CreateClient();
        var token = (await _userService.GetUserAsync()).Tokens?.FirstOrDefault(x => x.Provider == ProviderEnum.Genius.ToString());

        if (token is null)
            return "No Lyrics";

        UriBuilder uri = new(_baseApiUrl + _searchEndpointSuffix);
        uri.Query = new QueryBuilder
        {
            {"access_token", token.AccessToken },
            { "q", song.Name }
        }.ToString();

        SearchResponseModel? responseModel;
        var searchResponse = await client.GetAsync(uri.ToString());
        string response = await searchResponse.Content.ReadAsStringAsync();

        try
        {
            responseModel = JsonConvert.DeserializeObject<SearchResponseModel>(response);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        if (responseModel!.Meta.Status != 200 || responseModel is null || responseModel.Response.Hits.Count == 0)
            return "Failed to fetch";
        
        UriBuilder uriSongs = new(_baseApiUrl + responseModel.Response.Hits[0].Result.ApiPath);
        uriSongs.Query = new QueryBuilder
        {
            {"access_token", token.AccessToken },
            {"text_format", _textFormat }
        }.ToString();

        GetSongResponseModel? songResponseModel;
        var songResponse = await client.GetAsync(uriSongs.ToString());
        string responseContent = await songResponse.Content.ReadAsStringAsync();

        try
        {
            songResponseModel = JsonConvert.DeserializeObject<GetSongResponseModel>(responseContent);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        if (songResponseModel!.Meta.Status != 200)
            return "Failed to fetch song";

        return songResponseModel.Response.Song.EmbedContent;
    }
}