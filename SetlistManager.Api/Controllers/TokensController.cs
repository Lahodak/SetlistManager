using Microsoft.AspNetCore.Mvc;
using SetlistManager.Api.Services;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;


namespace SetlistManager.Api.Controllers;

[Route("api/tokens")]
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
        var userId = _userContext.GetCurrentUserId();

        if (userId is null)
            return BadRequest();

        UrlResponseModel model = new()
        {
            Url = await _geniusAuthService.GetGrantAccessTokenRequestUri(userId.Value)
        };

        return Ok(model);
    }
}