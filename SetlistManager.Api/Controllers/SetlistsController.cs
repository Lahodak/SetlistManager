using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;

namespace SetlistManager. Api.Controllers;
[Route("api/setlists")]

public class SetlistsController : BaseController
{
    private readonly ISetlistsService _setlistService;

    public SetlistsController(ISetlistsService setlistsDB)
    {
        _setlistService = setlistsDB;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SetlistModel>>> GetSetlists()
    {
        return Ok(await _setlistService.GetAllSetlistsAsync());
    }

    [HttpPost]
    public async Task<ActionResult> SaveSetlist(SetlistModel setlistModel)
    {
        await _setlistService.SaveSetlistAsync(setlistModel);
        return Created();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SetlistModel>> GetSetlistById(int id)
    {
        var result = await _setlistService.GetSetlistByIdAsync(id);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut]
    public async Task<ActionResult> EditSetlist(SetlistModel setlist)
    {
        if(setlist.Songs is null)
            return BadRequest();

        await _setlistService.EditSetlistAsync(setlist);

        return Ok();    
    }
}