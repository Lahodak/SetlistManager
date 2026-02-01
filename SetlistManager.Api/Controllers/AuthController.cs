using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SetlistManager.Api.Services;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Api.Controllers;

public class AuthController : BaseController
{
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;
    private readonly UserManager<User> _userManager;
    private readonly IAuthService _authService;

    public AuthController(UserManager<User> userManager, IJwtService jwtService, SignInManager<User> signInManager, IAuthService authService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _signInManager = signInManager;
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Register(RegisterRequestModel model)
    {
        var result = await _authService.RegisterAsync(model);

        if(!result.Success)
        {
            return BadRequest(result);
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
    public async Task<ActionResult<bool>> VerifyEmail(VerifyModel verifyModel)
    {
        var result = await _authService.VerifyEmailAsync(verifyModel);

        if(!result)
            return BadRequest(false);

        return Ok(true);
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword(ResetPasswordModel resetModel)
    {
        var result = await _authService.TryResetPasswordAsync(resetModel);

        if (!result)
            return BadRequest();

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("request-password-reset")]
    public async Task<ActionResult> RequestPasswordReset(PasswordResetRequestModel resetRequestModel)
    {
        var result = await _authService.RequestPasswordResetAsync(resetRequestModel);

        if(!result)
            return BadRequest();

        return NoContent();
    }
}