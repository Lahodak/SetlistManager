using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SetlistManager.Common.Genius.Models;
using SetlistManager.Data;
using SetlistManager.Data.Entities;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;
namespace SetlistManager.Business.Services;

public class TempAuthStorageService : ITempAuthStorageService
{
    private const string _geniusApiBaseUrl = "https://api.genius.com";
    private const string _authorizeEndpointSuffix = "/oauth/authorize";
    private const string _codeExchangeEndpointSuffix = "/oauth/token";
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    public TempAuthStorageService(AppDbContext dbContext, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
    }

    private async Task<string> CreateNewTempAuthSalt(int userId)
    {
        byte[] randomBytes = RandomNumberGenerator.GetBytes(16);
        string salt = Convert.ToBase64String(randomBytes);

        await _dbContext.TempAuthStorage.AddAsync(new(){
            UserId = userId,
            TempSalt = salt
        });
        await _dbContext.SaveChangesAsync();

        return salt;
    }

    public async Task<string> GetGrantAccessTokenRequestUri(int userId)
    {
        var grantModel = new GrantAccessTokenModel
        {
            ClientId = _configuration["Genius:ClientId"]!,
            RedirectUri = _configuration["Genius:GetGrantAccessTokenRequest:RedirectUri"]!,
            ResponseType = "code",
            Scope = "me",
            State = await CreateNewTempAuthSalt(userId)
        };

        UriBuilder uri = new (_geniusApiBaseUrl + _authorizeEndpointSuffix);
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

        try
        {
            jsonData = JsonConvert.SerializeObject(data);
        }
        catch (Exception ex)
        {
            return default;
        }

        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");

        var response = await client.PostAsync(uri.ToString(), content);

        CodeExchangeResponseModel? resultModel;
        var responseModel = await response.Content.ReadAsStringAsync();

        try
        {
            resultModel = JsonConvert.DeserializeObject<CodeExchangeResponseModel>(responseModel);
        }
        catch (Exception ex) 
        {
            return default;
        }

        return resultModel;
    }
}