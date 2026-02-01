using Microsoft.AspNetCore.Mvc;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

public class SetlistsController : BaseController
{
    private readonly ISetlistsService _setlistService;
    public SetlistsController(ISetlistsService setlistService)
    {
        _setlistService = setlistService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SetlistModel>>> GetSetlists([FromQuery] PagedRequest request)
    {
        var result = await _setlistService.GetSetlistsAsync(request);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SetlistModel>> GetSetlist(int id)
    {
        var result = await _setlistService.GetSetlistByIdAsync(id);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SetlistModel>> CreateSetlist([FromBody] SetlistModel setlist)
    {
        var result = await _setlistService.TryCreateSetlistAsync(setlist);
        
        if(!result)
            return BadRequest();

        return Created();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> EditSetlist(int id, [FromBody] SetlistModel setlist)
    {
        if (setlist.Songs is null)
            return BadRequest();

        await _setlistService.EditSetlistAsync(setlist);

        return NoContent();    
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> TryDeleteSetlist(int id)
    {
        if (!await _setlistService.TryDeleteSetlistAsync(id))
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/setlistsusers/{userId}")]
    public async Task<ActionResult> TryGiveAccessToSetlist(int id, int userId)
    {
        var result = await _setlistService.TryGiveAccessToSetlistAsync(id, userId);

        if (!result)
            return NotFound();

        return Created();
    }

    [HttpDelete("{id}/setlistsusers/{userId}")]
    public async Task<ActionResult> TryRemoveAccessFromUser(int id, int userId)
    {
        await _setlistService.RemoveAccessFromUserAsync(id, userId);

        return NoContent();
    }
}