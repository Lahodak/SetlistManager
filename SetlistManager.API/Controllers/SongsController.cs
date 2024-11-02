using Microsoft.AspNetCore.Mvc;
using SetlistManager.API.Data;
using SetlistManager.API.Models;

namespace SetlistManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class SongsController : ControllerBase
{
    private readonly ISongsDB _songsDB;

    public SongsController(ISongsDB songsDB)
    {
        _songsDB = songsDB;
    }

    [HttpGet]
    public async Task<IEnumerable<Song>> GetSongCollection()
    {
        return await _songsDB.GetSongsAsync();        
    }

    [HttpGet("{SongId}")]
    public async Task<ActionResult<Song>> GetSongById(int SongId)
    {
        var song = await _songsDB.GetSongByIdAsync(SongId);
        if (song is null)
        {
            return NotFound();
        }
        return song;
    }
}