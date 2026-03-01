using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;

namespace SetlistManager.Api.Controllers;

/// <summary>
/// Manages songs and user access to songs.
/// </summary>
public class SongsController : BaseController
{
    private readonly ISongService _songService;

    public SongsController(ISongService songService)
    {
        _songService = songService;
    }

    /// <summary>Gets a paginated list of songs filtered by content type.</summary>
    /// <param name="request">Pagination, search, and content-type parameters.</param>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<SongModel>>> GetSongs([FromQuery] ContentPagedRequest request)
    {
        return Ok(await _songService.GetSongsAsync(request));
    }

    /// <summary>Gets a song by its identifier.</summary>
    /// <param name="id">The song identifier.</param>
    [HttpGet("{id}")]
    public async Task<ActionResult<SongModel>> GetSongById(int id)
    {
        return Ok(await _songService.GetSongByIdAsync(id));
    }

    /// <summary>Creates a new song.</summary>
    /// <param name="createModel">The song creation payload.</param>
    [HttpPost]
    public async Task<ActionResult> CreateSong([FromBody] SongCreateModel createModel)
    {
        await _songService.CreateSongAsync(createModel);

        return Created();
    }

    /// <summary>Makes a song publicly visible to all users.</summary>
    /// <param name="id">The song identifier.</param>
    [HttpPost("{id}/public")]
    public async Task<ActionResult> MakeSongPublic(int id)
    {
        await _songService.MakeSongPublicAsync(id);
        
        return NoContent();
    }

    /// <summary>Grants a user access to a song.</summary>
    /// <param name="id">The song identifier.</param>
    /// <param name="userId">The user identifier to grant access to.</param>
    [HttpPost("{id}/users/{userId}")]
    public async Task<ActionResult> AddSongToUserLibrary(int id, int userId)
    {
        await _songService.GiveAccessToUserAsync(id, userId);
        
        return Created();
    }

    /// <summary>Revokes a user's access to a song.</summary>
    /// <param name="id">The song identifier.</param>
    /// <param name="userId">The user identifier to revoke access from.</param>
    [HttpDelete("{id}/users/{userId}")]
    public async Task<ActionResult> RemoveSongFromUserLibrary(int id, int userId)
    {
        await _songService.RemoveAccessFromUserAsync(id, userId);        
        
        return NoContent();
    }

    /// <summary>Updates an existing song.</summary>
    /// <param name="id">The song identifier.</param>
    /// <param name="updateModel">The updated song data.</param>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateSong(int id, [FromBody] SongUpdateModel updateModel)
    {
        await _songService.UpdateSongAsync(id, updateModel);
        
        return NoContent();
    }

    /// <summary>Deletes a song by its identifier.</summary>
    /// <param name="id">The song identifier.</param>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSong(int id)
    {
        await _songService.DeleteSongAsync(id);

        return NoContent();
    }
}