using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SetlistManager.Business.Options;
using SetlistManager.Business.Services;
using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

public class TokensController : BaseController
{
    private readonly ICurrentUserContext _userContext;    
    private readonly IGeniusAuthService _geniusAuthService;
    private readonly IUserService _userService;
    private readonly AppOptions _appOptions;

    public TokensController(ICurrentUserContext userContext, IGeniusAuthService geniusAuthService, IUserService userService, IOptions<AppOptions> appOptions)
    {        
        _userContext = userContext;
        _geniusAuthService = geniusAuthService;
        _userService = userService;
        _appOptions = appOptions.Value;
    }

    [HttpGet]
    public async Task<ActionResult<UrlResponseModel>> AuthorizeWithGenius()
    {
        var userId = _userContext.GetCurrentUserId()!.Value;

        UrlResponseModel model = new()
        {
            Url = await _geniusAuthService.GetGrantAccessTokenRequestUri(userId)
        };

        return Ok(model);
    }

    [AllowAnonymous]
    [HttpGet("genius/callback")]
    public async Task<ActionResult> AddGeniusTokenToUser([FromQuery] GrantAccessTokenResultModel grantResultModel)
    {
        await _userService.TryAddGeniusTokenToUserAsync(grantResultModel);

        return Redirect(_appOptions.UserPortalUrl);
    }
}