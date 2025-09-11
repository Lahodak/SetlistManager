using Microsoft.AspNetCore.Mvc;
using SetlistManager.API.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Controllers;
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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SetlistModel>> GetSetlistById(int id)
    {
        var result = await _setlistService.GetSetlistByIdAsync(id) ?? new SetlistModel();

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    //[HttpGet("{name:text}")]
    //public async Task<ActionResult<SetlistModel>> GetSetlistByName(string name)
    //{
    //    var setlist = await _setlistsDB.GetSetlistByNameAsync(name);

    //    if (setlist == null)
    //        return NotFound();
    //    return setlist;
    //}

    [HttpPut]
    public async Task<ActionResult> EditSetlist(SetlistModel setlist)
    {
        if(setlist == null)
            return BadRequest();

        await _setlistService.EditSetlistAsync(setlist);

        return Ok();    
    }
}