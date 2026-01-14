using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;

namespace SetlistManager.Api.Controllers;

[Route("api/setlists")]

public class SetlistsController : BaseController
{
    private readonly ISetlistsService _setlistService;

    public SetlistsController(ISetlistsService setlistService)
    {
        _setlistService = setlistService;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> EditSetlist(int id, [FromBody] SetlistModel setlist)
    {
        if(setlist.Songs is null)
            return BadRequest();

        await _setlistService.EditSetlistAsync(setlist);

        return NoContent();    
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSetlist(int id)
    {
        if(!await _setlistService.TryDeleteSetlistAsync(id))
            return NotFound();

        return NoContent();
    }
}