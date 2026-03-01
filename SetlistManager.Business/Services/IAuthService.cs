using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

/// <summary>
/// Provides authentication and account management operations.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user account and sends a verification email.
    /// </summary>
    /// <param name="model">The registration details.</param>
    Task RegisterAsync(RegisterRequestModel model);

    /// <summary>
    /// Verifies a user's email address using a confirmation token.
    /// </summary>
    /// <param name="verifyModel">The email and verification token.</param>
    Task VerifyEmailAsync(VerifyModel verifyModel);

    /// <summary>
    /// Resets a user's password using a reset token.
    /// </summary>
    /// <param name="resetModel">The email, new password, and reset token.</param>
    Task ResetPasswordAsync(ResetPasswordModel resetModel);

    /// <summary>
    /// Sends a password-reset token to the specified email address.
    /// </summary>
    /// <param name="resetRequestModel">The email to send the reset link to.</param>
    Task RequestPasswordResetAsync(PasswordResetRequestModel resetRequestModel);

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="model">The login credentials.</param>
    /// <returns>A result containing the JWT token.</returns>
    Task<LoginResultModel> LoginAsync(LoginRequestModel model);
}