using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;

namespace SetlistManager.Api.Controllers;

[Route("api/artists")]
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
        return Ok(await _artistService.GetAllArtistsAsync(request));
    }

    [HttpPost]
    public async Task<ActionResult> UploadArtist(ArtistCreateModel createModel)
    {
        if (await _artistService.UploadArtistAsync(createModel))
            return NoContent();

        return BadRequest();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ArtistModel>> GetArtistById(int id)
    {        
        return Ok(await _artistService.GetArtistByIdAsync(id));
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteArtist(int id)
    {
        if(await _artistService.TryDeleteArtistAsync(id))
            return Ok();

        return BadRequest();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateArtist(int id, ArtistUpdateModel updateModel)
    {
        if (await _artistService.TryUpdateArtistAsync(id, updateModel))
            return Ok();

        return BadRequest();
    }
}