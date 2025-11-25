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
    public async Task<ActionResult<IEnumerable<ArtistModel>>> GetAllArtists()
    {
        return Ok(await _artistService.GetAllArtistsAsync());
    }

    [HttpPost]
    public async Task<ActionResult> UploadArtist(ArtistCreateModel createModel)
    {
        if (await _artistService.UploadArtistAsync(createModel))
            return NoContent();

        return BadRequest();
    }

    [HttpGet("{artistId}")]
    public async Task<ActionResult<ArtistModel>> GetArtistById(int artistId)
    {        
        return Ok(await _artistService.GetArtistByIdAsync(artistId));
    }
    
    [HttpDelete("{artistId}")]
    public async Task<ActionResult> DeleteArtist(int artistId)
    {
        if(await _artistService.TryDeleteArtistAsync(artistId))
            return Ok();

        return BadRequest();
    }

    [HttpPut("{artistId}")]
    public async Task<ActionResult> UpdateArtist(int artistId, ArtistUpdateModel updateModel)
    {
        if (await _artistService.TryUpdateArtistAsync(artistId, updateModel))
            return Ok();

        return BadRequest();
    }
}