using Microsoft.AspNetCore.Http.Extensions;
using SetlistManager.Common.Models;
using SetlistManager.Common.Genius.Models.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.App.Models;
using SetlistManager.Common.Exceptions;
using System.Net.Http.Json;

namespace SetlistManager.App.Services.Implementations;

public class GeniusService : IGeniusService
{
    private const string _searchEndpointSuffix = "/search?";
    private const string _geniusAccessTokenKey = "access_token";
    private const string _geniusQueryKey = "q";
    private readonly IUserService _userService;
    private readonly IApiService _apiService;
    private readonly GeniusOptions _geniusOptions;
    private readonly SetlistManagerApiOptions _apiOptions;
    private readonly HttpClient _client;
    
    public GeniusService(HttpClient client, IOptions<GeniusOptions> geniusOptions, IUserService userService, IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiService = apiService;
        _apiOptions = apiOptions.Value;
        _geniusOptions = geniusOptions.Value;
        _client = client;
        _userService = userService;
    }

    public async Task<string> AuthorizeAsync()
    {
        var response = await _apiService.GetAsync<UrlResponseModel>(_apiOptions.BaseUrl + _apiOptions.TokensEndpoint);

        if (response is null)
            return "/error";

        return response.Url;
    }

    public async Task<GeniusEmbedModel?> FetchSongLyricsAsync(SongModel song)
    {
        var token = (await _userService.GetUserAsync())?.Tokens?.FirstOrDefault(x => x.Provider == ProviderEnum.Genius.ToString());

        if (token is null)
            return null;

        UriBuilder uri = new(_geniusOptions.BaseApiUrl + _searchEndpointSuffix)
        {
            Query = new QueryBuilder
            {
                { _geniusAccessTokenKey, token.AccessToken },
                { _geniusQueryKey, song.Name }
            }.ToString()
        };

        var searchResponse = await _client.GetFromJsonAsync<SearchResponseModel>(uri.ToString());

        if (searchResponse is null || searchResponse.Meta.Status != StatusCodes.Status200OK || searchResponse.Response.Hits.Count == 0)
            throw new GeniusSongLyricsNotFoundException();   

        var result = searchResponse.Response.Hits[0].Result;

        return new GeniusEmbedModel
        {
            SongId = result.Id.ToString(),
            Title = result.Title,
            Artist = result.PrimaryArtistNames,
            Url = result.Url
        };
    }
}