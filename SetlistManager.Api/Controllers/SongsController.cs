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
        var userId = _userContext.GetCurrentUserId();

        var result = await _songService.GetSongsAsync(request, userId!.Value);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SongModel>> GetSongById(int id)
    {
        var userId = _userContext.GetCurrentUserId();

        var result = await _songService.GetSongByIdAsync(id, userId!.Value);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateSong(SongCreateModel createModel)
    {
        var userId = _userContext.GetCurrentUserId();

        if(!await _songService.TryCreateSongAsync(createModel, userId!.Value))
            return BadRequest("Song already exists");

        return Created();
    }

    [HttpPost("{id}/public")]
    public async Task<ActionResult> TryMakeSongPublic(int songId)
    {
        var userId = _userContext.GetCurrentUserId();

        var result = await _songService.TryMakeSongPublicAsync(songId, userId!.Value);
        
        if (!result)
            return BadRequest("Song is already in user's library");

        return NoContent();
    }

    [HttpPost("{id}/songsusers/{userId}")]
    public async Task<ActionResult> AddSongToUserLibrary(int id, int userId)
    {
        var currentUserId = _userContext.GetCurrentUserId();
        
        var result = await _songService.TryGiveAccessToUserAsync(id, userId, currentUserId!.Value);
        
        if (!result)        
            return BadRequest("Song is already in user's library");        
        
        return Created();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSong(int id, SongUpdateModel updateModel)
    {
        var userId = _userContext.GetCurrentUserId();

        var result = await _songService.TryUpdateSongAsync(id, updateModel, userId!.Value);
        
        if (!result)        
            return NotFound();        
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSong(int id)
    {
        var userId = _userContext.GetCurrentUserId();

        var result = await _songService.TryDeleteSongAsync(id, userId!.Value);

        if (!result)        
            return NotFound();        
        
        return NoContent();
    }
}