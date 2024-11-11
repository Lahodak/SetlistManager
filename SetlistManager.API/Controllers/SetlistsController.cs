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
        => await _setlistsDB.SaveSetlist(setlistModel);

    [HttpGet("{id}")]
    public async Task<SetlistModel> GetSetlistById(int id) 
        => await _setlistsDB.GetSetlistById(id) ?? new SetlistModel();
}