using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;

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
        return Ok(await _songService.GetSongsAsync(request));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SongModel>> GetSongById(int id)
    {
        return Ok(await _songService.GetSongByIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult> CreateSong([FromBody] SongCreateModel createModel)
    {
        await _songService.TryCreateSongAsync(createModel);

        return Created();
    }

    [HttpPost("{id}/public")]
    public async Task<ActionResult> MakeSongPublic(int id)
    {
        await _songService.TryMakeSongPublicAsync(id);
        
        return NoContent();
    }

    [HttpPost("{id}/users/{userId}")]
    public async Task<ActionResult> AddSongToUserLibrary(int id, int userId)
    {
        await _songService.TryGiveAccessToUserAsync(id, userId);
        
        return Created();
    }

    [HttpDelete("{id}/users/{userId}")]
    public async Task<ActionResult> RemoveSongFromUserLibrary(int id, int userId)
    {
        await _songService.RemoveAccessFromUserAsync(id, userId);        
        
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSong(int id, [FromBody] SongUpdateModel updateModel)
    {
        await _songService.TryUpdateSongAsync(id, updateModel);
        
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSong(int id)
    {
        await _songService.TryDeleteSongAsync(id);

        return NoContent();
    }
}