using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SetlistManager.Business.Options;
using SetlistManager.Common.Genius.Models;
using System.Net.Http.Json;

namespace SetlistManager.Business.Services.Implementations;

public class GeniusAuthService : IGeniusAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITempAuthStorageService _tempAuthStorageService;
    private readonly IOptions<GeniusOptions> _geniusOptions;
    private readonly ILogger<GeniusAuthService> _logger;

    private const string _authorizeEndpointSuffix = "/oauth/authorize";
    private const string _codeExchangeEndpointSuffix = "/oauth/token";

    public GeniusAuthService(IOptions<GeniusOptions> geniusOptions, IHttpClientFactory httpClientFactory, ITempAuthStorageService tempAuthStorageService, ILogger<GeniusAuthService> logger)
    {
        _geniusOptions = geniusOptions;         
        _httpClientFactory = httpClientFactory;
        _tempAuthStorageService = tempAuthStorageService;
        _logger = logger;
    }

    public async Task<string> GetGrantAccessTokenRequestUri(int userId)
    {
        var grantModel = new GrantAccessTokenModel
        {
            ClientId = _geniusOptions.Value.ClientId,
            RedirectUri = _geniusOptions.Value.GetGrantAccessTokenRequest.RedirectUri,
            ResponseType = "code",
            Scope = "me",
            State = await _tempAuthStorageService.CreateNewTempAuthSecret(userId)
        };

        UriBuilder uri = new(_geniusOptions.Value.ApiBaseUrl + _authorizeEndpointSuffix)
        {
            Query = new QueryBuilder
            {
                { "client_id", grantModel.ClientId },
                { "redirect_uri", grantModel.RedirectUri },
                { "scope", grantModel.Scope },
                { "state", grantModel.State },
                { "response_type", grantModel.ResponseType }
            }.ToString()
        };

        return uri.ToString();
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