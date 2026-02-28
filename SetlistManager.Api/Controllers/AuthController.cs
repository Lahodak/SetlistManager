using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Register(RegisterRequestModel model)
    {
        await _authService.RegisterAsync(model);

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResultModel>> Login(LoginRequestModel model)
    {
        return Ok(await _authService.LoginAsync(model));
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<ActionResult> VerifyEmail(VerifyModel verifyModel)
    {
        await _authService.VerifyEmailAsync(verifyModel);

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword(ResetPasswordModel resetModel)
    {
        await _authService.ResetPasswordAsync(resetModel);

        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("request-password-reset")]
    public async Task<ActionResult> RequestPasswordReset(PasswordResetRequestModel resetRequestModel)
    {
        await _authService.RequestPasswordResetAsync(resetRequestModel);

        return NoContent();
    }

    [HttpPost("verify-token")]
    public ActionResult VerifyToken()
    {
        return NoContent();
    }
}