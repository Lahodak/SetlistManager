using Microsoft.AspNetCore.Mvc;
using SetlistManager.API.Data;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SetlistsController : ControllerBase
{
    private readonly ISetlistsDB _setlistsDB;

    public SetlistsController(ISetlistsDB setlistsDB)
    {
        _setlistsDB = setlistsDB;
    }

    [HttpPost]
    public async Task<int> UploadSetlistToDb(SetlistModel setlistModel)
        => await _setlistsDB.SaveSetlistAsync(setlistModel);

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SetlistModel>> GetSetlistById(int id)
    {
        var result = await _setlistsDB.GetSetlistByIdAsync(id) ?? new SetlistModel();


        if (result == null)
            return NotFound();

        return result;
    }

    [HttpGet("usersetlists/{UserId:int}")]
    public async Task<ActionResult<List<SetlistModel>>> GetUserSetlistsByUserId(int id)
    {
        var result = await _setlistsDB.GetAllSetlistsOfUserAsync(id);


        if (result == null)
            return NotFound();

        return null;
    }

    [HttpGet("getallsetlists")]
    public async Task<ActionResult<List<SetlistModel>>> GetAllSetlists()
    {
        var result = await _setlistsDB.GetAllSetlistsAsync();


        if (result == null)
            return NotFound();

        return result.ToList();
    }

    [HttpGet("{setlistName}")]
    public async Task<ActionResult<SetlistModel>> GetSetlistByName(string setlistName)
    {
        var result = await _setlistsDB.GetSetlistByNameAsync(setlistName) ?? new SetlistModel();

        if (result == null)
            return NotFound();

        return result;
    }

    [HttpPost("editsetlist")]
    public async Task<ActionResult> EditSetlist(SetlistModel setlist)
    {
        if(setlist == null)
            return BadRequest();

        if (!await _setlistsDB.EditSetlistAsync(setlist)) 
            return BadRequest();

        return Ok();    
    }
}