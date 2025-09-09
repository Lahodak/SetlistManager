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
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly UserService _userService;
    private readonly IJwtService _jwtService;
    private ISetlistsDB _setlistsDB;

    public UsersController(SignInManager<User> signInManager, UserManager<User> userManager, UserService userService, IJwtService jwtService, 
        ISetlistsDB setlistsDB)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _userService = userService;
        _jwtService = jwtService;
        _setlistsDB = setlistsDB;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Register(RegisterRequestModel model)
    {
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return BadRequest(new RegisterResultModel
            {
                Success = false,
                Message = "User with this email already exists."
            });
        }

        var existingUserByName = await _userManager.FindByNameAsync(model.UserName);
        if (existingUserByName != null)
        {
            return BadRequest(new RegisterResultModel
            {
                Success = false,
                Message = "Username is already taken."
            });
        }

        var user = new User
        {
            UserName = model.UserName,
            Email = model.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new RegisterResultModel
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            });
        }

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("auth")]
    public async Task<ActionResult<LoginResultModel>> Login(LoginRequestModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)
        {
            return Unauthorized("User with provided email does not exist.");
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

        if (result.IsLockedOut)
        {
            return Unauthorized("User account is locked out.");
        }
        else if (result.IsNotAllowed)
        {
            return Unauthorized("User is not allowed to sign in.");
        }
        else if (!result.Succeeded)
        {
            return Unauthorized("Invalid login attempt.");
        }

        var token = await _jwtService.GenerateTokenAsync(user);

        return Ok(new LoginResultModel { Token = token });
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