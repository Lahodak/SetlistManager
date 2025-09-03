using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SetlistManager.API.Data.Entities;
using SetlistManager.API.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Controllers;

public class IdentityController : BaseController
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;

    public IdentityController(SignInManager<User> signInManager, UserManager<User> userManager, IJwtService jwtService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
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
    [HttpPost("login")]
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
}