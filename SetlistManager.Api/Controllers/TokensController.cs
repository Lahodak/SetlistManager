using Microsoft.AspNetCore.Mvc;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

public class TokensController : BaseController
{
    private readonly ICurrentUserContext _userContext;    
    private readonly IGeniusAuthService _geniusAuthService;

    public TokensController(ICurrentUserContext userContext, IGeniusAuthService geniusAuthService)
    {        
        _userContext = userContext;
        _geniusAuthService = geniusAuthService;
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
}