using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SetlistManager.Api.Services;
using SetlistManager.Business.Options;
using SetlistManager.Business.Services;
using SetlistManager.Common.Genius.Models;
using SetlistManager.Common.Models;

namespace SetlistManager. Api.Controllers;

[Route("api/users")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly ISetlistsService _setlistsService;
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IGeniusAuthService _geniusAuthService;
    private readonly IOptions<AppOptions> _appOptions;
    private readonly IArtistService _artistService;
    private readonly ISongService _songService;

    public UsersController(IUserService userService, ISetlistsService setlistsService, ICurrentUserContext currentUserContext,
        IGeniusAuthService geniusAuthService, IOptions<AppOptions> appOptions, IArtistService artistService, ISongService songService)
    {
        _appOptions = appOptions;
        _userService = userService;
        _setlistsService = setlistsService;
        _currentUserContext = currentUserContext;
        _geniusAuthService = geniusAuthService;
        _artistService = artistService;
        _songService = songService;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(UserModel model)
    {
        await _userService.UpdateUserAsync(model);
        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<UserViewModel>>> GetUsers([FromQuery] PagedRequest pagedRequest)
    {
        return Ok(await _userService.GetUsersAsync(pagedRequest));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserModel>> GetUserById(int id)
    {
        var userId = _currentUserContext.GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        return Ok(await _userService.GetCurrentUserAsync(userId.Value));
    }

    [HttpPost("{id}/setlists")]
    public async Task<ActionResult<SetlistModel>> CreateUserSetlist(int id, [FromBody] SetlistModel createModel)
    {
        var result = await _setlistsService.TryCreateSetlistAsync(createModel, id);

        if (!result)
            return BadRequest("Could not create setlist for user");

        return Created();
    }

    //[HttpPost("{targetId}/setlistsusers/{setlistId}")]
    //public async Task<ActionResult> GiveSetlistAccessToUser(int targetId, int setlistId)
    //{
    //    var result = await _setlistsService.TryGiveAccessToSetlistAsync(setlistId, targetId);

    //    if (!result)
    //        return BadRequest();

    //    return Created();
    //}

    [HttpPut("{id}/setlists/{setlistId}")]
    public async Task<ActionResult> UpdateUserSetlist(int id, int setlistId, [FromBody] SetlistModel updateModel)
    {
        await _setlistsService.EditSetlistAsync(updateModel);        
        return NoContent();
    }

    [HttpDelete("{id}/setlists/{setlistId}")]
    public async Task<ActionResult> DeleteUserSetlist(int id, int setlistId)
    {
        await _setlistsService.TryDeleteSetlistAsync(setlistId);        
        return NoContent();
    }

    [HttpGet("{id}/setlists/{setlistId}")]
    public async Task<ActionResult<SetlistModel>> GetUserSetlistById(int id, int setlistId)
    {
        var setlist =  await _setlistsService.GetSetlistByIdAsync(setlistId);

        if (setlist is null)
            return NotFound("Setlist not found");
        
        return Ok(setlist);
    }

    [HttpGet("{id}/setlists")]
    public async Task<ActionResult<PagedResponse<SetlistModel>>> GetUserSetlists(int id, [FromQuery] PagedRequest request)
    {
        return Ok(await _setlistsService.GetUserSetlistsLibraryAsync(id, request));
    }

    [HttpGet("{id}/artists")]
    public async Task<ActionResult<PagedResponse<ArtistModel>>> GetUserArtists(int id, [FromQuery] PagedRequest request)
    {
        return Ok(await _artistService.GetUserArtistLibraryAsync(request, id));
    }

    [HttpGet("{id}/artists/{artistId}")]
    public async Task<ActionResult<ArtistModel>> GetArtistDetail(int id, int artistId)
    {
        var artist = await _artistService.GetUserArtistById(artistId, id);

        if (artist is null)
            return NotFound();

        return Ok(artist);
    }

    [HttpPost("{id}/artists")]
    public async Task<ActionResult> TryCreatePrivateArtist(int id, ArtistCreateModel createModel)
    {
        var result = await _artistService.TryCreateArtistAsync(createModel, id);

        if (!result)
            return BadRequest();

        return Created();
    }

    [HttpDelete("{id}/artists/{artistId}")]
    public async Task<ActionResult> TryDeleteArtist(int id, int artistId)
    {
        var result = await _artistService.TryDeleteArtistAsync(artistId, id);

        if (!result)
            return BadRequest();

        return NoContent();
    }

    [HttpPut("{id}/artists/{artistId}")]
    public async Task<ActionResult> TryUpdateArtist(int id, int artistId, [FromBody] ArtistUpdateModel updateModel)
    {
        var result = await _artistService.TryUpdateArtistAsync(artistId, updateModel);

        if (!result)
            BadRequest();

        return NoContent();
    }

    [HttpPost("{id}/artists/{artistId}/make-public")]
    public async Task<ActionResult> TryMakeArtistPublic(int id, int artistId)
    {
        var result = await _artistService.TryMakeArtistPublicAsync(artistId);

        if (!result)
            BadRequest();

        return NoContent();
    }

    //[HttpPost("{targetId}/artists")]

    [AllowAnonymous]
    [HttpGet("tokens")]
    public async Task<ActionResult> AddUserToken([FromQuery] GrantAccessTokenResultModel grantResultModel)
    {
        if (grantResultModel is null)
            return BadRequest();

        var user = await _userService.GetUserByTempSalt(grantResultModel.State);

        if (user is null)
            return NotFound("User not found");

        var resultAccessTokenModel = await _geniusAuthService.ExchangeGeniusCode(grantResultModel.Code);

        if (resultAccessTokenModel is null || resultAccessTokenModel.AccessToken is null)
            return BadRequest();

        TokenCreateModel tokenModel = new()
        {
            Provider = ProviderEnum.Genius,
            AccessToken = resultAccessTokenModel.AccessToken,
            RefreshToken = null
        };

        var result = await _userService.TryAddUserTokenAsync(user.Id, tokenModel);
        
        if(!result)
            return BadRequest("Could not add token to user, provider not found");

        return Redirect(_appOptions.Value.UserPortalUrl);
    }

    [HttpGet("{id}/songs")]
    public async Task<ActionResult<PagedResponse<SongModel>>> GetUserSongsLibrary(int id, [FromQuery] PagedRequest request)
    {
        return Ok(await _songService.GetSongLibraryByUserId(id, request));
    }

    [HttpGet("{id}/songs/{songId}")]
    public async Task<ActionResult<SongModel>> GetUserSongDetail(int id, int songId)
    {
        return Ok(await _songService.GetUserSongById(id, songId));
    }

    [HttpPost("{id}/friendships")]
    public async Task<ActionResult> InitiateFriendship(int id, [FromBody] FriendshipRequestModel requestModel)
    {
        await _userService.HandleFriendshipRequestAsync(id, requestModel);
        
        return NoContent();
    }

    [HttpDelete("{id}/friendships/{friendshipId}")]
    public async Task<ActionResult> RemoveFriendship(int id, int friendshipId)
    {
        await _userService.RemoveFriendshipAsync(id, friendshipId);
        
        return NoContent();
    }

    [HttpGet("{id}/friendships")]
    public async Task<ActionResult<PagedResponse<FriendModel>>> GetUserFriends(int id, [FromQuery] PagedRequest pagedRequest)
    {
        var currentUserId = _currentUserContext.GetCurrentUserId();
        
        if (currentUserId is null)
            return Unauthorized();
        
        var result = await _userService.GetUserFriendsAsync(id, pagedRequest);
                
        return Ok(result);
    }

    [HttpPut("{id}/friendships/{friendshipId}")]
    public async Task<ActionResult> AcceptFriendship(int id, int friendshipId)
    {        
        await _userService.AcceptFriendshipAsync(id, friendshipId);
        
        return NoContent();
    }
}