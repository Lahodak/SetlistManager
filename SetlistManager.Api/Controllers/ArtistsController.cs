using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;

namespace SetlistManager.Api.Controllers;

/// <summary>
/// Manages artists and user access to artists.
/// </summary>
public class ArtistsController : BaseController
{
    private readonly IArtistService _artistService;

    public ArtistsController(IArtistService artistService)
    {
        _artistService = artistService;
    }

    /// <summary>Gets a paginated list of artists filtered by content type.</summary>
    /// <param name="request">Pagination, search, and content-type parameters.</param>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ArtistModel>>> GetAllArtists([FromQuery] ContentPagedRequest request)
    {
        return Ok(await _artistService.GetArtistsAsync(request));
    }

    /// <summary>Gets an artist by its identifier.</summary>
    /// <param name="id">The artist identifier.</param>
    /// <param name="contentType">The visibility scope to apply.</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<ArtistModel>> GetArtistById(int id, [FromQuery] ContentType contentType)
    {                        
        return Ok(await _artistService.GetArtistByIdAsync(id, contentType));
    }

    /// <summary>Creates a new artist.</summary>
    /// <param name="createModel">The artist creation payload.</param>
    [HttpPost]
    public async Task<ActionResult> CreateArtist(ArtistCreateModel createModel)
    {
        await _artistService.CreateArtistAsync(createModel);       

        return Created();
    }

    /// <summary>Updates an existing artist.</summary>
    /// <param name="id">The artist identifier.</param>
    /// <param name="updateModel">The updated artist data.</param>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateArtist(int id, ArtistUpdateModel updateModel)
    {
        await _artistService.UpdateArtistAsync(id, updateModel);
        
        return NoContent();
    }

    /// <summary>Deletes an artist by its identifier.</summary>
    /// <param name="id">The artist identifier.</param>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteArtist(int id)
    {
        await _artistService.DeleteArtistAsync(id);
        
        return NoContent();
    }

    /// <summary>Makes an artist publicly visible to all users.</summary>
    /// <param name="id">The artist identifier.</param>
    [HttpPost("{id}/public")]
    public async Task<ActionResult> MakeArtistPublic(int id)
    {
        await _artistService.MakeArtistPublicAsync(id);

        return NoContent();        
    }

    /// <summary>Grants a user access to an artist.</summary>
    /// <param name="id">The artist identifier.</param>
    /// <param name="userId">The user identifier to grant access to.</param>
    [HttpPost("{id}/users/{userId}")]
    public async Task<ActionResult> GiveAccessToUser(int id, int userId)
    {
        await _artistService.GiveAccessToUserAsync(id, userId);

        return NoContent();
    }

    /// <summary>Revokes a user's access to an artist.</summary>
    /// <param name="id">The artist identifier.</param>
    /// <param name="userId">The user identifier to revoke access from.</param>
    [HttpDelete("{id}/users/{userId}")]
    public async Task<ActionResult> RemoveAccessFromUser(int id, int userId)
    {
        await _artistService.RemoveAccessFromUserAsync(id, userId);

        return NoContent();
    }
}