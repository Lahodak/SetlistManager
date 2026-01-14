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
    public async Task<ActionResult<PagedResponse<SongModel>>> GetSongs([FromQuery] PagedRequest request)
    {
        var result = await _songService.GetPublicSongsAsync(request);

        if (result is null)        
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> AddSong(SongCreateModel createModel)
    {
        var userId = _userContext.GetCurrentUserId();

        if(!await _songService.TrySaveSongAsync(createModel, userId!.Value))
            return BadRequest("Song already exists");

        return Created();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SongModel>> GetSongById(int id)
    {
        var song = await _songService.GetPublicSongByIdAsync(id);

        if (song is null)        
            return NotFound();        

        return Ok(song);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSong(int id, SongUpdateModel updateModel)
    {
        var userId = _userContext.GetCurrentUserId();

        var success = await _songService.TryUpdateSongAsync(id, updateModel, userId!.Value);
        
        if (!success)        
            return NotFound();        
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSong(int id)
    {
        var success = await _songService.TryDeleteSongAsync(id);

        if (!success)        
            return NotFound();        
        
        return NoContent();
    }
}