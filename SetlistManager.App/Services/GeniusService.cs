using SetlistManager.Common.Genius.Models;

namespace SetlistManager.App.Services;

public class GeniusService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public const string _geniusApiBaseUrl = "https://api.genius.com";
    public const string _authorizeEndpointSuffix = "/oauth/authorize";
    private const string _clientId = "ADCrJAva4a0yZGi4AhCqtjAOkEiYLrHf-lYqB3LstfPUIb-Y6VBiO0-tbeCgv_QS";
    private const string _clientSecret = "DbZaK1XNnoukW5dNDqJCO_IVjFFAjwiU-5KpKSUv5Z_4XbWda1PF4R2UcJtGF7k48uoADFlhXEfKFsLvvonjyA";
    private readonly string _grantAccessTokenRequestRedirectUri;
    public GeniusService(IHttpClientFactory factory, IConfiguration configuration)
    {
        _httpClientFactory = factory;
        _grantAccessTokenRequestRedirectUri = configuration["SetlistManager.Api:UsersEndpoint"]!;
    }

    public string GetGrantAccessTokenRequestUri()
    {
        var grantModel = new GrantAccessTokenModel
        {
            ClientId = _clientId,
            RedirectUri = _grantAccessTokenRequestRedirectUri,
            ResponseType = "code",
            Scope = "me"
        };

        string redirectUri = $"{_geniusApiBaseUrl}{_authorizeEndpointSuffix}?{grantModel.ClientId}&{grantModel.RedirectUri}&{grantModel.Scope}&{grantModel.State}&{grantModel.ResponseType}";
        return redirectUri;
    }    
}