using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SetlistManager.Business.Options;
using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;
using System.Net.Http.Json;

namespace SetlistManager.Business.Services.Implementations;

public class GeniusAuthService : IGeniusAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITempAuthStorageService _tempAuthStorageService;
    private readonly IOptions<GeniusOptions> _geniusOptions;
    private readonly ILogger<GeniusAuthService> _logger;
    private readonly ICurrentUserContext _userContext;

    private const string _authorizeEndpointSuffix = "/oauth/authorize";
    private const string _codeExchangeEndpointSuffix = "/oauth/token";
    private const string _scope = "me";
    private const string _responseType = "code";
    private const string _clientIdKey = "client_id";
    private const string _redirectUriKey = "redirect_uri";
    private const string _scopeKey = "scope";
    private const string _stateKey = "state";
    private const string _responseTypeKey = "response_type";

    public GeniusAuthService(IOptions<GeniusOptions> geniusOptions, IHttpClientFactory httpClientFactory, ITempAuthStorageService tempAuthStorageService, ILogger<GeniusAuthService> logger, ICurrentUserContext userContext)
    {
        _geniusOptions = geniusOptions;
        _httpClientFactory = httpClientFactory;
        _tempAuthStorageService = tempAuthStorageService;
        _logger = logger;
        _userContext = userContext;
    }

    public async Task<UrlResponseModel> GetGrantAccessTokenRequestUri()
    {
        var userId = _userContext.GetCurrentUserId()!.Value;

        GrantAccessTokenModel grantModel = new()
        {
            ClientId = _geniusOptions.Value.ClientId,
            RedirectUri = _geniusOptions.Value.GetGrantAccessTokenRequest.RedirectUri,
            ResponseType = _responseType,
            Scope = _scope,
            State = await _tempAuthStorageService.CreateNewTempAuthSecret(userId)
        };

        UriBuilder uri = new(_geniusOptions.Value.ApiBaseUrl + _authorizeEndpointSuffix)
        {
            Query = new QueryBuilder
            {
                { _clientIdKey, grantModel.ClientId },
                { _redirectUriKey, grantModel.RedirectUri },
                { _scopeKey, grantModel.Scope },
                { _stateKey, grantModel.State },
                { _responseTypeKey, grantModel.ResponseType }
            }.ToString()
        };

        return new()
        {
            Url = uri.ToString()
        };
    }

    public async Task<CodeExchangeResponseModel?> ExchangeGeniusCode(string code)
    {
        CodeExchangeRequestModel data = new()
        {
            ClientId = _geniusOptions.Value.ClientId,
            ClientSecret = _geniusOptions.Value.ClientSecret,
            Code = code,
            RedirectUri = _geniusOptions.Value.GetGrantAccessTokenRequest.RedirectUri
        };
        UriBuilder uri = new(_geniusOptions.Value.ApiBaseUrl + _codeExchangeEndpointSuffix);

        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync(uri.ToString(), data);

        var result = await response.Content.ReadFromJsonAsync<CodeExchangeResponseModel>();

        if (result is null)
            _logger.Log(LogLevel.Error, "Failed to read Genius code");

        return result;
    }
}