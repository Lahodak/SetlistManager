using Microsoft.AspNetCore.Http.Extensions;
using SetlistManager.Common.Genius.Models;

namespace SetlistManager.App.Services;

public class GeniusService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public const string _geniusApiBaseUrl = "https://api.genius.com";
    public const string _authorizeEndpointSuffix = "/oauth/authorize";
    private const string _clientId = "";
    private const string _clientSecret = "";
    private readonly UserService _userService;
    private readonly string _grantAccessTokenRequestRedirectUri;
    public GeniusService(IHttpClientFactory factory, IConfiguration configuration, UserService userService)
    {
        _httpClientFactory = factory;
        _userService = userService;
        _grantAccessTokenRequestRedirectUri = configuration["SetlistManager.Api:UsersEndpoint"]!;
    }

    public async Task<string> GetGrantAccessTokenRequestUri()
    {
        var grantModel = new GrantAccessTokenModel
        {
            ClientId = _clientId,
            RedirectUri = _grantAccessTokenRequestRedirectUri + "/tokens",
            ResponseType = "code",
            Scope = "me",
            State = (await _userService.GetUserAsync()).Id.ToString()
        };

        UriBuilder uri = new UriBuilder(_geniusApiBaseUrl + _authorizeEndpointSuffix );
        uri.Query = new QueryBuilder
        {
            { "client_id", grantModel.ClientId },
            { "redirect_uri", grantModel.RedirectUri },
            { "scope", grantModel.Scope },
            { "state", grantModel.State },
            { "response_type", grantModel.ResponseType }
        }.ToString();

        return uri.ToString();
    }    
}