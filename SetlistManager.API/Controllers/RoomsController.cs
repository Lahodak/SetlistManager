using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.API.Data.Entities;
using SetlistManager.API.Services;
namespace SetlistManager.API.Controllers;

[Route("api/rooms")]

public class RoomsController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly IRoomsService _roomsService;
    private readonly ICurrentUserContext _currentUserContext;

    public RoomsController(UserManager<User> userManager, IRoomsService roomsService, ICurrentUserContext currentUserContext)
    {
        _userManager = userManager;
        _roomsService = roomsService;
        _currentUserContext = currentUserContext;
    }

    [HttpPost("join")]
    public async Task<ActionResult<RoomModel>> JoinRoomAsync(JoinRoomModel joinRoomModel)
    {
        var userId = _currentUserContext.GetCurrentUserId();        

        var user = await _userManager.FindByIdAsync(userId.ToString()!);
        
        if (user is null)
            return BadRequest("Couldn't find user");

        RoomModel roomModel;

        try
        {
            roomModel = await _roomsService.JoinRoomAsync(joinRoomModel, user);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);            
        }

        return Ok(roomModel);
    }

    [HttpPut]
    public async Task<ActionResult> ChangeSongAsync(ChangeCurrentSongModel changeCurrentModel)
    {        
        var userId = _currentUserContext.GetCurrentUserId();

        if (changeCurrentModel.AdminId != userId)
            return Unauthorized("User is not Room Admin");

        try
        {
            await _roomsService.ChangeCurrentSongAsync(changeCurrentModel);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<RoomModel>> CreateRoomAsync(RoomModel roomModel)
    {
        roomModel.HostId = (int)_currentUserContext.GetCurrentUserId()!;
        await _roomsService.CreateRoomAsync(roomModel);        

        return Ok();
    }
}