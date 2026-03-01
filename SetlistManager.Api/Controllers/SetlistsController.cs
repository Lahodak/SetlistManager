using Microsoft.AspNetCore.Mvc;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

/// <summary>
/// Manages setlists and user access to setlists.
/// </summary>
public class SetlistsController : BaseController
{
    private readonly ISetlistsService _setlistService;
    public SetlistsController(ISetlistsService setlistService)
    {
        _setlistService = setlistService;
    }

    /// <summary>Gets a paginated list of setlists.</summary>
    /// <param name="request">Pagination and search parameters.</param>
    [HttpGet]
    public async Task<ActionResult<List<SetlistModel>>> GetSetlists([FromQuery] PagedRequest request)
    {
        return Ok(await _setlistService.GetSetlistsAsync(request));
    }

    /// <summary>Gets a setlist by its identifier.</summary>
    /// <param name="id">The setlist identifier.</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<SetlistModel>> GetSetlist(int id)
    {
        return Ok(await _setlistService.GetSetlistByIdAsync(id));
    }

    /// <summary>Creates a new setlist.</summary>
    /// <param name="setlist">The setlist creation payload.</param>
    [HttpPost]
    public async Task<ActionResult> CreateSetlist([FromBody] SetlistCreateModel setlist)
    {
        await _setlistService.CreateSetlistAsync(setlist);
        
        return Created();
    }

    /// <summary>Updates an existing setlist, including its song ordering.</summary>
    /// <param name="id">The setlist identifier.</param>
    /// <param name="setlist">The updated setlist data. Must include songs.</param>
    [HttpPut("{id}")]
    public async Task<ActionResult> EditSetlist(int id, [FromBody] SetlistModel setlist)
    {
        if (setlist.Songs is null)
            return BadRequest();

        await _setlistService.EditSetlistAsync(setlist);

        return NoContent();    
    }

    /// <summary>Deletes a setlist by its identifier.</summary>
    /// <param name="id">The setlist identifier.</param>
    [HttpDelete("{id}")]
    public async Task<ActionResult> TryDeleteSetlist(int id)
    {
        await _setlistService.DeleteSetlistAsync(id);

        return NoContent();
    }

    /// <summary>Grants a user access to a setlist.</summary>
    /// <param name="id">The setlist identifier.</param>
    /// <param name="userId">The user identifier to grant access to.</param>
    [HttpPost("{id}/users/{userId}")]
    public async Task<ActionResult> TryGiveAccessToSetlist(int id, int userId)
    {
        await _setlistService.GiveAccessToSetlistAsync(id, userId);

        return Created();
    }

    /// <summary>Revokes a user's access to a setlist.</summary>
    /// <param name="id">The setlist identifier.</param>
    /// <param name="userId">The user identifier to revoke access from.</param>
    [HttpDelete("{id}/users/{userId}")]
    public async Task<ActionResult> TryRemoveAccessFromUser(int id, int userId)
    {
        await _setlistService.RemoveAccessFromUserAsync(id, userId);

        return NoContent();
    }
}