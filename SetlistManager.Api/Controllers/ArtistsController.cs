using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;
using SetlistManager.Api.Services;

namespace SetlistManager.Api.Controllers;

[Route("api/artists")]
public class ArtistsController : BaseController
{
    private readonly IArtistService _artistService;
    private readonly ICurrentUserContext _userContext;

    public ArtistsController(IArtistService artistService, ICurrentUserContext userContext)
    {
        _artistService = artistService;
        _userContext = userContext;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ArtistModel>>> GetAllArtists([FromQuery] PagedRequest request)
    {
        var userId = _userContext.GetCurrentUserId();
        
        return Ok(await _artistService.GetArtistsAsync(request, userId!.Value));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArtistModel>> GetArtistById(int id, [FromQuery] ContentType contentType)
    {        
        var userId = _userContext.GetCurrentUserId();
        var result = await _artistService.GetArtistByIdAsync(id, userId!.Value, contentType);
        
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateArtist(ArtistCreateModel createModel)
    {
        var userId = _userContext.GetCurrentUserId();
        var result = await _artistService.TryCreateArtistAsync(createModel, userId!.Value);
        
        if(!result)
            return BadRequest();

        return Created();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateArtist(int id, ArtistUpdateModel updateModel)
    {
        var userId = _userContext.GetCurrentUserId();
        
        if (await _artistService.TryUpdateArtistAsync(id, updateModel, userId!.Value))
            return NoContent();

        return BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteArtist(int id)
    {
        var userId = _userContext.GetCurrentUserId();
        
        if (await _artistService.TryDeleteArtistAsync(id, userId!.Value))
            return NoContent();

        return BadRequest();
    }

    [HttpPost("{id}/public")]
    public async Task<ActionResult> MakeArtistPublic(int id)
    {
        var userId = _userContext.GetCurrentUserId();
        
        if (await _artistService.TryMakeArtistPublicAsync(id, userId!.Value))
            return NoContent();
        
        return BadRequest();
    }

    [HttpPost("{id}/artistsusers/{userId}")]
    public async Task<ActionResult> GiveAccessToUser(int id, int userId)
    {
        var currentUserId = _userContext.GetCurrentUserId();

        if (await _artistService.TryGiveAccessToUserAsync(id, userId, currentUserId))
            return NoContent();
        
        return BadRequest();
    }
}