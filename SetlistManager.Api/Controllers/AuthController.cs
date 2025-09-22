using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SetlistManager. Api.Services;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager. Api.Controllers;

[Route("api/auth")]

public class AuthController : BaseController
{
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly UserManager<User> _userManager;
    private readonly IMailService _mailService;

    public AuthController(UserManager<User> userManager, IJwtService jwtService, SignInManager<User> signInManager, IMailService mailService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _signInManager = signInManager;
        _mailService = mailService;
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
            Email = model.Email
        };


        var result = await _userManager.CreateAsync(user, model.Password);

        var createdUser = await _userManager.FindByEmailAsync(user.Email);

        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(createdUser);
        await _mailService.SendVerificationEmailAsync(user.Email, confirmationToken);

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

    [AllowAnonymous]
    [HttpPost("verify")]
    public async Task<ActionResult> VerifyEmail(VerifyEmailModel verifyModel)
    {
        var user = await _userManager.FindByEmailAsync(verifyModel.Email);

        if (user is null)
            return Unauthorized();

        var x = await _userManager.ConfirmEmailAsync(user, verifyModel.Token);

        if (x.Succeeded)
            return Ok();

        return BadRequest();
    }
}