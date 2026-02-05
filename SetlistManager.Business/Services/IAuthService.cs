using SetlistManager.Common.Models;

namespace SetlistManager.Business.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequestModel model);
    Task VerifyEmailAsync(VerifyModel verifyModel);
    Task TryResetPasswordAsync(ResetPasswordModel resetModel);
    Task RequestPasswordResetAsync(PasswordResetRequestModel resetRequestModel);
    Task<LoginResultModel> LoginAsync(LoginRequestModel model);
}