namespace SetlistManager.Business.Services;

/// <summary>
/// Defines methods for sending email notifications related to user account management, such as verification and
/// password reset emails.
/// </summary>
public interface IMailService
{
    /// <summary>
    /// Sends a verification email to the specified address with the provided email verification token.
    /// </summary>
    /// <param name="email">The email address to which the verification email will be sent.</param>
    /// <param name="token">The token used to verify the email address</param>
    Task SendVerificationEmailAsync(string email, string token);

    /// <summary>
    /// Sends a password reset email to the specified address using the provided reset token.
    /// </summary>
    /// <param name="email">The email address to which the password reset message will be sent.</param>
    /// <param name="token">The token used to authorize the password reset request. This token must be valid and unexpired.</param>
    Task SendPasswordResetEmailAsync(string email, string token);
}
