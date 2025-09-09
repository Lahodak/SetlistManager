using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.API.Data.Entities;
using System.Security.Claims;
using SetlistManager.API.Data;
using Microsoft.AspNetCore.Routing.Constraints;
namespace SetlistManager.API.Controllers;

[Route("api/rooms")]

public class RoomsController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly IRoomsDB _roomsDB;

    public RoomsController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult<RoomModel>> JoinRoomAsync(JoinRoomModel joinRoomModel)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("Invalid token: User ID not found");

        var userId = userIdClaim.Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest("Couldn't find user");

        RoomModel roomModel;
        try
        {
            roomModel = await _roomsDB.JoinRoomAsync(joinRoomModel, user);
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("Invalid token: User ID not found");

        var userId = userIdClaim.Value;
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return BadRequest("Couldn't find user");

        if (int.TryParse(userId, out int id))
            return BadRequest("UserId is not a number");

        if (changeCurrentModel.AdminId != id)
            return Unauthorized("User is not Room Admin");

        try
        {
            await _roomsDB.ChangeCurrentSongAsync(changeCurrentModel);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public async Task<ActionResult<RoomModel>> CreateRoomAsync()
    {
        //To-Do

        return BadRequest("ok");
    }
}