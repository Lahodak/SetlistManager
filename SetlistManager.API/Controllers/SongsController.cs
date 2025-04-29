using Microsoft.AspNetCore.Mvc;
using SetlistManager.API.Data;
using SetlistManager.API.Entities;
using SetlistManager.API.Models;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public partial class SongsController : ControllerBase
{
    private readonly ISongsDB _songsDB;

    public SongsController(ISongsDB songsDB)
    {
        _songsDB = songsDB;
    }

    [HttpGet]
    public async Task<IEnumerable<SongModel>> GetSongCollection()
    {
        return await _songsDB.GetSongsAsync();        
    }

    [HttpPost]
    public async Task AddSongs(AddSongsModel addSongs)
    {
        foreach(var song in addSongs.Songs)
        {
            await _songsDB.UploadSongs(new()
            {
                Name = song.Name,
                Artist = song.Artist,
                YouTubeURL = song.YouTubeURL,
                TabsURL = song.TabsURL,
                Language = song.Language
            });
        }
    }

    [HttpGet("{SongId}")]
    public async Task<ActionResult<SongModel>> GetSongById(int SongId)
    {
        var song = await _songsDB.GetSongByIdAsync(SongId);
        if (song is null)
        {
            return NotFound();
        }
        return song;
    }
}