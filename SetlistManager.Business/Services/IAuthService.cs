using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequestModel model);
    Task<bool> VerifyEmailAsync(VerifyModel verifyModel);
    Task<bool> TryResetPasswordAsync(ResetPasswordModel resetModel);
    Task<bool> RequestPasswordResetAsync(PasswordResetRequestModel resetRequestModel);
}