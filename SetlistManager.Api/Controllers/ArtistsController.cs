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
    public async Task<ActionResult<PagedResponse<ArtistModel>>> GetAllArtists([FromQuery] ContentPagedRequest request)
    {
        return Ok(await _artistService.GetArtistsAsync(request));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArtistModel>> GetArtistById(int id, [FromQuery] ContentType contentType)
    {                        
        return Ok(await _artistService.GetArtistByIdAsync(id, contentType));
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
        await _artistService.TryUpdateArtistAsync(id, updateModel);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteArtist(int id)
    {
        await _artistService.TryDeleteArtistAsync(id);
        
        return NoContent();
    }

    [HttpPost("{id}/public")]
    public async Task<ActionResult> MakeArtistPublic(int id)
    {
        await _artistService.TryMakeArtistPublicAsync(id);

        return NoContent();        
    }

    [HttpPost("{id}/users/{userId}")]
    public async Task<ActionResult> GiveAccessToUser(int id, int userId)
    {
        await _artistService.TryGiveAccessToUserAsync(id, userId);

        return NoContent();
    }

    [HttpDelete("{id}/users/{userId}")]
    public async Task<ActionResult> RemoveAccessFromUser(int id, int userId)
    {
        await _artistService.RemoveAccessFromUserAsync(id, userId);

        return NoContent();
    }
}