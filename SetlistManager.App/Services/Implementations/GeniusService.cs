using Microsoft.AspNetCore.Http.Extensions;
using SetlistManager.Common.Models;
using Newtonsoft.Json;
using SetlistManager.Common.Genius.Models.Search;
using SetlistManager.Common.Genius.Models.Songs;
using Microsoft.AspNetCore.Http;

namespace SetlistManager.App.Services.Implementations;

public class GeniusService : IGeniusService
{
    private const string _baseApiUrl = "https://api.genius.com"; //do konfigurace
    private const string _searchEndpointSuffix = "/search?";
    private const string _textFormat = "html";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserService _userService;
    public GeniusService(IHttpClientFactory factory, IConfiguration configuration, IUserService userService)
    {
        _httpClientFactory = factory;
        _userService = userService;
    }

    public async Task<string?> FetchSongLyricsAsync(SongModel song)
    {
        var client = _httpClientFactory.CreateClient();
        var token = (await _userService.GetUserAsync())?.Tokens?.FirstOrDefault(x => x.Provider == ProviderEnum.Genius.ToString());

        if (token is null)
            return null;

        UriBuilder uri = new(_baseApiUrl + _searchEndpointSuffix);
        uri.Query = new QueryBuilder
        {
            { "access_token", token.AccessToken },
            { "q", song.Name }
        }.ToString();

        SearchResponseModel? responseModel;
        var searchResponse = await client.GetAsync(uri.ToString());
        string response = await searchResponse.Content.ReadAsStringAsync();

        try
        {
            responseModel = JsonConvert.DeserializeObject<SearchResponseModel>(response);
        }
        catch (Exception)
        {
            return null;
        }

        if (responseModel is null || responseModel.Meta.Status != StatusCodes.Status200OK || responseModel.Response.Hits.Count == 0)
            return null;
        
        UriBuilder uriSongs = new(_baseApiUrl + responseModel.Response.Hits[0].Result.ApiPath);
        uriSongs.Query = new QueryBuilder
        {
            { "access_token", token.AccessToken },
            { "text_format", _textFormat }
        }.ToString();

        GetSongResponseModel? songResponseModel;
        var songResponse = await client.GetAsync(uriSongs.ToString());
        string responseContent = await songResponse.Content.ReadAsStringAsync();

        try
        {
            songResponseModel = JsonConvert.DeserializeObject<GetSongResponseModel>(responseContent);
        }
        catch (Exception)
        {
            return null;
        }

        if (songResponseModel is null || songResponseModel.Meta.Status != StatusCodes.Status200OK)
            return null;

        return songResponseModel.Response.Song.EmbedContent;
    }
}