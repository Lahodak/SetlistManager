using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManger.Business.Services;

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
        => Ok(await _setlistService.GetAllSetlistsAsync());

    [HttpPost]
    public async Task UploadSetlistToDb(SetlistModel setlistModel)
        => await _setlistService.SaveSetlistAsync(setlistModel);

    [HttpGet("{id}")]
    public async Task<ActionResult<SetlistModel>> GetSetlistById(int id)
    {
        var result = await _setlistService.GetSetlistByIdAsync(id) ?? new SetlistModel();

        if (result == null)
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