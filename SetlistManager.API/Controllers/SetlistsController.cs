using Microsoft.AspNetCore.Mvc;
using SetlistManager.API.Data;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Controllers;
[Route("api/setlists")]

public class SetlistsController : BaseController
{
    private readonly ISetlistsDB _setlistsDB;

    public SetlistsController(ISetlistsDB setlistsDB)
    {
        _setlistsDB = setlistsDB;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SetlistModel>>> GetSetlists([FromQuery] int? userId, [FromQuery] string? name)
    {
        if (userId.HasValue)
        {
            var userSetlists = await _setlistsDB.GetAllSetlistsOfUserAsync(userId.Value);
            return userSetlists?.ToList() ?? [];
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var setlist = await _setlistsDB.GetSetlistByNameAsync(name);
            
            if (setlist == null) 
                return NotFound();
            
            return new List<SetlistModel> 
            { 
                setlist 
            };
        }

        var allSetlists = await _setlistsDB.GetAllSetlistsAsync();
        return allSetlists?.ToList() ?? [];
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

    [HttpPut]
    public async Task<ActionResult> EditSetlist(SetlistModel setlist)
    {
        if(setlist == null)
            return BadRequest();

        if (!await _setlistsDB.EditSetlistAsync(setlist)) 
            return BadRequest();

        return Ok();    
    }
}