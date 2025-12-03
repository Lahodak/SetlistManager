using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SetlistManager.Business.Options;
using SetlistManager.Common.Genius.Models;

namespace SetlistManager.Business.Services.Implementations;

public class GeniusAuthService : IGeniusAuthService
{
    private const string _authorizeEndpointSuffix = "/oauth/authorize";
    private const string _codeExchangeEndpointSuffix = "/oauth/token";
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITempAuthStorageService _tempAuthStorageService;
    private readonly IOptions<GeniusOptions> _geniusOptions;

    public GeniusAuthService(IOptions<GeniusOptions> geniusOptions, IHttpClientFactory httpClientFactory, ITempAuthStorageService tempAuthStorageService)
    {
        _geniusOptions = geniusOptions;         
        _httpClientFactory = httpClientFactory;
        _tempAuthStorageService = tempAuthStorageService;
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

        string jsonData = JsonConvert.SerializeObject(data);

        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync(uri.ToString(), content);

        CodeExchangeResponseModel? resultModel;
        var responseModel = await response.Content.ReadAsStringAsync();

        try
        {
            resultModel = JsonConvert.DeserializeObject<CodeExchangeResponseModel>(responseModel);
        }
        catch (Exception)
        {
            return null;
        }

        return resultModel;
    }
}