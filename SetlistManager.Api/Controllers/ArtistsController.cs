using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;

namespace SetlistManager.Api.Controllers;

public class ArtistsController : BaseController
{
    private readonly IArtistService _artistService;

    public ArtistsController(IArtistService artistService)
    {
        _artistService = artistService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<ArtistModel>>> GetAllArtists([FromQuery] PagedRequest request)
    {
        return Ok(await _artistService.GetArtistsAsync(request));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArtistModel>> GetArtistById(int id, [FromQuery] ContentType contentType)
    {        
        var result = await _artistService.GetArtistByIdAsync(id, contentType);
        
        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateArtist(ArtistCreateModel createModel)
    {
        await _artistService.TryCreateArtistAsync(createModel);       

        return Created();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateArtist(int id, ArtistUpdateModel updateModel)
    {        
        if (await _artistService.TryUpdateArtistAsync(id, updateModel))
            return NoContent();

        return BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteArtist(int id)
    {        
        if (await _artistService.TryDeleteArtistAsync(id))
            return NoContent();

        return BadRequest();
    }

    [HttpPost("{id}/public")]
    public async Task<ActionResult> MakeArtistPublic(int id)
    {        
        if (await _artistService.TryMakeArtistPublicAsync(id))
            return NoContent();
        
        return BadRequest();
    }

    [HttpPost("{id}/users/{userId}")]
    public async Task<ActionResult> GiveAccessToUser(int id, int userId)
    {
        if (await _artistService.TryGiveAccessToUserAsync(id, userId))
            return NoContent();
        
        return BadRequest();
    }

    [HttpDelete("{id}/users/{userId}")]
    public async Task<ActionResult> RemoveAccessFromUser(int id, int userId)
    {
        await _artistService.RemoveAccessFromUserAsync(id, userId);

        return NoContent();
    }
}