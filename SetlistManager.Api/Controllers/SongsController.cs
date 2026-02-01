using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;
using SetlistManager.Api.Services;

namespace SetlistManager.Api.Controllers;

public class SongsController : BaseController
{
    private readonly ISongService _songService;

    public SongsController(ISongService songService)
    {
        _songService = songService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<SongModel>>> GetSongs([FromQuery] PagedRequest request)
    {
        var result = await _songService.GetSongsAsync(request);        

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SongModel>> GetSongById(int id)
    {
        var result = await _songService.GetSongByIdAsync(id);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult> CreateSong([FromBody] SongCreateModel createModel)
    {
        if(!await _songService.TryCreateSongAsync(createModel))
            return BadRequest("Song already exists");

        return Created();
    }

    [HttpPost("{id}/public")]
    public async Task<ActionResult> MakeSongPublic(int id)
    {
        var result = await _songService.TryMakeSongPublicAsync(id);
        
        if (!result)
            return BadRequest("Song is already in user's library");

        return NoContent();
    }

    [HttpPost("{id}/songsusers/{userId}")]
    public async Task<ActionResult> AddSongToUserLibrary(int id, int userId)
    {
        var result = await _songService.TryGiveAccessToUserAsync(id, userId);
        
        if (!result)        
            return BadRequest("Song is already in user's library");        
        
        return Created();
    }

    [HttpDelete("{id}/songsusers/{userId}")]
    public async Task<ActionResult> RemoveSongFromUserLibrary(int id, int userId)
    {
        await _songService.RemoveAccessFromUserAsync(id, userId);        
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSong(int id, [FromBody] SongUpdateModel updateModel)
    {
        var result = await _songService.TryUpdateSongAsync(id, updateModel);
        
        if (!result)        
            return NotFound();        
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSong(int id)
    {
        var result = await _songService.TryDeleteSongAsync(id);

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