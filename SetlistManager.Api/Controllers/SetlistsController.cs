using Microsoft.AspNetCore.Mvc;
using SetlistManager.Api.Services;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

[Route("api/setlists")]

public class SetlistsController : BaseController
{
    private readonly ISetlistsService _setlistService;
    private readonly ICurrentUserContext _userContext;
    
    public SetlistsController(ISetlistsService setlistService, ICurrentUserContext userContext)
    {
        _setlistService = setlistService;
        _userContext = userContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<SetlistModel>>> GetSetlists([FromQuery] PagedRequest request)
    {
        var userId = _userContext.GetCurrentUserId();

        var result = await _setlistService.GetSetlistsAsync(userId!.Value, request);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SetlistModel>> GetSetlist(int id)
    {
        var userId = _userContext.GetCurrentUserId();

        var result = await _setlistService.GetSetlistByIdAsync(id, userId!.Value);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SetlistModel>> CreateSetlist([FromBody] SetlistModel setlist)
    {
        var userId = _userContext.GetCurrentUserId();        

        var result = await _setlistService.TryCreateSetlistAsync(setlist, userId!.Value);
        
        if(!result)
            return BadRequest();

        return Created();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> TryEditSetlist(int id, [FromBody] SetlistModel setlist)
    {
        var currentUserId = _userContext.GetCurrentUserId();

        if (setlist.Songs is null)
            return BadRequest();

        await _setlistService.EditSetlistAsync(setlist, currentUserId!.Value);

        return NoContent();    
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> TryDeleteSetlist(int id)
    {
        var currentUserId = _userContext.GetCurrentUserId();

        if (!await _setlistService.TryDeleteSetlistAsync(id, currentUserId!.Value))
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/setlistsusers/{userId}")]
    public async Task<ActionResult> TryGiveAccessToSetlist(int id, int userId)
    {
        var currentUserId = _userContext.GetCurrentUserId();

        var result = await _setlistService.TryGiveAccessToSetlistAsync(id, userId, currentUserId!.Value);

        if (!result)
            return NotFound();

        return Created();
    }
}