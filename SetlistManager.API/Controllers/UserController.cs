using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SetlistManager.API.Data;
using SetlistManager.API.Data.Entities;
using SetlistManager.API.Services;
using SetlistManager.Common.Models;
using System.Security.Claims;

namespace SetlistManager.API.Controllers;

public class UserController : BaseController
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly UserService _userService;
    public UserController(SignInManager<User> signInManager, UserManager<User> userManager, UserService userService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userService = userService;
    }

    [HttpPost("updateuser")]
    public async Task<ActionResult> UpdateUser(UserModel model)
    {
        await _userService.UpdateUser(model);
        return Ok();
    }

    [HttpGet("getuserdetail")]
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
}