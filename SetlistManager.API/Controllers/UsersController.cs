using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SetlistManager.API.Data;
using SetlistManager.API.Data.Entities;
using SetlistManager.API.Services;
using SetlistManager.Common.Models;
using System.Security.Claims;

namespace SetlistManager.API.Controllers;
[Route("api/users")]
public class UsersController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly UserService _userService;
    private ISetlistsDB _setlistsDB;

    public UsersController(UserManager<User> userManager, UserService userService, ISetlistsDB setlistsDB)
    {
        _userManager = userManager;
        _userService = userService;
        _setlistsDB = setlistsDB;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(UserModel model)
    {
        await _userService.UpdateUser(model);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<UserModel>> GetUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized("Invalid token: User ID not found");

        var userId = userIdClaim.Value;
        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return NotFound();

        return Ok(user.ToModel());
    }

    [HttpGet("{id:int}/setlists")]
    public async Task<ActionResult<List<SetlistModel>>> GetUserSetlists(int id)
    {
        return (await _setlistsDB.GetAllSetlistsOfUserAsync(id)).ToList();
    }
}