namespace SetlistManager.Business.Services;

public interface IMailService
{
    Task SendVerificationEmailAsync(string email, string token);
    Task SendPasswordResetEmailAsync(string email, string token);
}
