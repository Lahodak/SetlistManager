using Microsoft.AspNetCore.Mvc;
using SetlistManager.Data.Entities;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

[Route("api/artists")]
public class ArtistsController : BaseController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArtistModel>>> GetAllArtists()
    {

        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult> UploadArtist(ArtistModel artistModel)
    {

        return Ok();
    }

    [HttpGet("{artistId}")]
    public async Task<ActionResult<ArtistModel>> GetArtistById(int artistId)
    {

        return Ok();
    }
}