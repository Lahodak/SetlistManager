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
        return Ok(await _setlistService.GetSetlistsAsync(request));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SetlistModel>> GetSetlist(int id)
    {
        return Ok(await _setlistService.GetSetlistByIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult> CreateSetlist([FromBody] SetlistCreateModel setlist)
    {
        await _setlistService.CreateSetlistAsync(setlist);
        
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
        await _setlistService.DeleteSetlistAsync(id);

        return NoContent();
    }

    [HttpPost("{id}/users/{userId}")]
    public async Task<ActionResult> TryGiveAccessToSetlist(int id, int userId)
    {
        await _setlistService.GiveAccessToSetlistAsync(id, userId);

        return Created();
    }

    [HttpDelete("{id}/users/{userId}")]
    public async Task<ActionResult> TryRemoveAccessFromUser(int id, int userId)
    {
        await _setlistService.RemoveAccessFromUserAsync(id, userId);

        return NoContent();
    }
}