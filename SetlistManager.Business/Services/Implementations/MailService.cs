using Microsoft.Extensions.Options;
using SetlistManager.Business.Options;
using SetlistManager.Resources.Storage;
using System.Text;
using System.Text.Json;

namespace SetlistManager.Business.Services.Implementations;

public class MailService : IMailService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<BrevoOptions> _brevoOptions;

    public MailService(IHttpClientFactory httpClientFactory, IOptions<BrevoOptions> brevoOptions)
    {
        _httpClientFactory = httpClientFactory;
        _brevoOptions = brevoOptions;
    }

    public async Task SendVerificationEmailAsync(string email, string token)
    {
        var subject = "Verify your Setlist Manager account";
        var verificationLink = $"https://localhost:7025/verify-email?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

        var body = $@"
        <p>Please click the button below to verify your account:</p>
        <p><a href='{verificationLink}' style='
            display:inline-block;
            padding:10px 20px;
            background-color:#4CAF50;
            color:white;
            text-decoration:none;
            border-radius:5px;'>
            Verify Email
        </a></p>
        <p>If you cannot click the button, copy and paste this link into your browser:</p>
        <p>{verificationLink}</p>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string email, string token)
    {
        var subject = "Reset your Setlist Manager password";
        var verificationLink = $"https://localhost:7025/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

        var body = $@"
        <p>Please click the button below to reset your password:</p>
        <p><a href='{verificationLink}' style='
            display:inline-block;
            padding:10px 20px;
            background-color:#4CAF50;
            color:white;
            text-decoration:none;
            border-radius:5px;'>
            Reset Password
        </a></p>
        <p>If you cannot click the button, copy and paste this link into your browser:</p>
        <p>{verificationLink}</p>";

        var mail = Storage.ResetPasswordMail;

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string recipientEmail, string subject, string htmlContent)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("api-key", _brevoOptions.Value.ApiKey);

        var payload = new
        {
            sender = new { name = _brevoOptions.Value.SenderName, email = _brevoOptions.Value.SenderEmail },
            to = new[] { new { email = recipientEmail } },
            subject,
            htmlContent
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(_brevoOptions.Value.SmtpApi, content);
        response.EnsureSuccessStatusCode();
    }
}