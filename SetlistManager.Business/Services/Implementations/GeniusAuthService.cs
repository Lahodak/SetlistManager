using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SetlistManager.Common.Genius.Models;

namespace SetlistManager.Business.Services.Implementations;

public class GeniusAuthService : IGeniusAuthService
{
    private const string _authorizeEndpointSuffix = "/oauth/authorize";
    private const string _codeExchangeEndpointSuffix = "/oauth/token";
    private readonly string _geniusApiBaseUrl;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ITempAuthStorageService _tempAuthStorageService;

    public GeniusAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ITempAuthStorageService tempAuthStorageService)
    {
        _configuration = configuration;
        _geniusApiBaseUrl = _configuration["Genius:ApiBaseUrl"]!;
        _httpClientFactory = httpClientFactory;
        _tempAuthStorageService = tempAuthStorageService;
    }

    public async Task<string> GetGrantAccessTokenRequestUri(int userId)
    {
        var grantModel = new GrantAccessTokenModel
        {
            ClientId = _configuration["Genius:ClientId"]!,
            RedirectUri = _configuration["Genius:GetGrantAccessTokenRequest:RedirectUri"]!,
            ResponseType = "code",
            Scope = "me",
            State = await _tempAuthStorageService.CreateNewTempAuthSecret(userId)
        };

        UriBuilder uri = new(_geniusApiBaseUrl + _authorizeEndpointSuffix)
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
            ClientId = _configuration["Genius:ClientId"]!,
            ClientSecret = _configuration["Genius:ClientSecret"]!,
            Code = code,
            RedirectUri = _configuration["Genius:GetGrantAccessTokenRequest:RedirectUri"]!
        };
        UriBuilder uri = new(_geniusApiBaseUrl + _codeExchangeEndpointSuffix);

        var client = _httpClientFactory.CreateClient();

        string jsonData;

        jsonData = JsonConvert.SerializeObject(data);

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
            return default;
        }

        return resultModel;
    }
}