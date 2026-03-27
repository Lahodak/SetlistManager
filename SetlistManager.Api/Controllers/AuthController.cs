using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

/// <summary>
/// Handles user registration, login, email verification, and password reset.
/// </summary>
public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Registers a new user account.</summary>
    /// <param name="model">The registration request payload.</param>
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult> Register(RegisterRequestModel model)
    {
        await _authService.RegisterAsync(model);

        return NoContent();
    }

    /// <summary>Authenticates a user and returns a JWT token.</summary>
    /// <param name="model">The login credentials.</param>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResultModel>> Login(LoginRequestModel model)
    {
        return Ok(await _authService.LoginAsync(model));
    }

    /// <summary>Verifies a user's email address using a confirmation token.</summary>
    /// <param name="verifyModel">The email and verification token.</param>
    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<ActionResult> VerifyEmail(VerifyModel verifyModel)
    {
        await _authService.VerifyEmailAsync(verifyModel);

        return NoContent();
    }

    /// <summary>Resets a user's password using a reset token.</summary>
    /// <param name="resetModel">The email, new password, and reset token.</param>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword(ResetPasswordModel resetModel)
    {
        await _authService.ResetPasswordAsync(resetModel);

        return NoContent();
    }

    /// <summary>Sends a password-reset token to the specified email address.</summary>
    /// <param name="resetRequestModel">The email to send the reset link to.</param>
    [AllowAnonymous]
    [HttpPost("request-password-reset")]
    public async Task<ActionResult> RequestPasswordReset(PasswordResetRequestModel resetRequestModel)
    {
        await _authService.RequestPasswordResetAsync(resetRequestModel);

        return NoContent();
    }

    /// <summary>Validates the caller's JWT token. Returns 204 if valid.</summary>
    [HttpPost("verify-token")]
    public ActionResult VerifyToken()
    {
        return NoContent();
    }
}