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
    public async Task<ActionResult> CreateSong([FromBody] SongCreateModel createModel)
    {
        var userId = _userContext.GetCurrentUserId();

        if(!await _songService.TryCreateSongAsync(createModel, userId!.Value))
            return BadRequest("Song already exists");

        return Created();
    }

    [HttpPost("{id}/public")]
    public async Task<ActionResult> TryMakeSongPublic(int id)
    {
        var userId = _userContext.GetCurrentUserId();

        var result = await _songService.TryMakeSongPublicAsync(id, userId!.Value);
        
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

    [HttpDelete("{id}/songsusers/{userId}")]
    public async Task<ActionResult> RemoveSongFromUserLibrary(int id, int userId)
    {
        var currentUserId = _userContext.GetCurrentUserId();        
        await _songService.RemoveAccessFromUserAsync(id, userId, currentUserId!.Value);        
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSong(int id, [FromBody] SongUpdateModel updateModel)
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

    [HttpGet("most-used")]
    public async Task<ActionResult<PagedResponse<SongUsageStatModel>>> MostUsed([FromQuery] StatsPagedRequest request)
    {
        return Ok(await _songService.GetMostUsedSongsAsync(request));
    }

    [HttpGet("most-added")]
    public async Task<ActionResult<PagedResponse<SongUsageStatModel>>> MostAdded([FromQuery] PagedRequest request)
    {
        return Ok(await _songService.GetMostAddedToLibraryAsync(request));
    }

    [HttpGet("latest-public")]
    public async Task<ActionResult<PagedResponse<LatestSongStatModel>>> LatestPublic([FromQuery] PagedRequest request)
    {
        return Ok(await _songService.GetLatestPublicSongsAsync(request));
    }
}