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
    public async Task<ActionResult> UploadArtist(ArtistModel artistModel)
    {
        await _artistService.UploadArtistAsync(artistModel);
        return Ok();
    }

    [HttpGet("{artistId}")]
    public async Task<ActionResult<ArtistModel>> GetArtistById(int artistId)
    {
        return Ok(await _artistService.GetArtistModelByIdAsync(artistId));
    }
}