using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;
using SetlistManager.Api.Services;

namespace SetlistManager.Api.Controllers;

[Route("api/songs")]

public class SongsController : BaseController
{
    private readonly ISongService _songService;
    private readonly ICurrentUserContext _userContext;

    public SongsController(ISongService songService, ICurrentUserContext userContext)
    {
        _songService = songService;
        _userContext = userContext;
    }

    [HttpGet]
    public async Task<ActionResult<List<SongModel>>> GetSongs([FromQuery] string? name)
    {
        List<SongModel>? songs;

        if (string.IsNullOrWhiteSpace(name))
        {
            songs = await _songService.GetSongsAsync();
        }
        else
        {
            songs = await _songService.GetSongByNameAsync(name);
        }

        if (songs is null)        
            return NotFound();        

        return Ok(songs);
    }

    [HttpPost]
    public async Task AddSong(SongCreateModel createModel)
    {
        var userId = _userContext.GetCurrentUserId();

        await _songService.UploadSongAsync(createModel, userId!.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SongModel>> GetSongById(int id)
    {
        var song = await _songService.GetSongByIdAsync(id);

        if (song is null)
        {
            return NotFound();
        }

        return Ok(song);
    }
}