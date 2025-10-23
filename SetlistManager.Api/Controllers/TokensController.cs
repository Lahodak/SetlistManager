using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SetlistManager.Api.Services;
using SetlistManager.Business.Services;
using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;


namespace SetlistManager.Api.Controllers;

[Route("api/tokens")]
public class TokensController : BaseController
{
    private readonly ICurrentUserContext _userContext;
    private readonly ITempAuthStorageService _tempAuthStorageService;
    public TokensController(ICurrentUserContext userContext, ITempAuthStorageService tempAuthStorageService)
    {        
        _userContext = userContext;
        _tempAuthStorageService = tempAuthStorageService;
    }

    [HttpGet]
    public async Task<ActionResult<UrlResponseModel>> AuthorizeWithGenius()
    {
        var userId = _userContext.GetCurrentUserId();

        if (userId is null)
            return BadRequest();

        UrlResponseModel model = new()
        {
            Url = await _tempAuthStorageService.GetGrantAccessTokenRequestUri(userId.Value)
        };

        return Ok(model);
    }
}