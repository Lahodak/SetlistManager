using Microsoft.AspNetCore.Http.Extensions;
using SetlistManager.Common.Genius.Models;

namespace SetlistManager.App.Services;

public class GeniusService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UserService _userService;
    private readonly string _grantAccessTokenRequestRedirectUri;
    public GeniusService(IHttpClientFactory factory, IConfiguration configuration, UserService userService)
    {
        _httpClientFactory = factory;
        _userService = userService;
        _grantAccessTokenRequestRedirectUri = configuration["SetlistManager.Api:UsersEndpoint"]!;
    } 
}