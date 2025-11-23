using Microsoft.AspNetCore.Http.Extensions;
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
        UriBuilder uri = new(_brevoOptions.Value.VerifyEmailRedirect)
        {
            Query = new QueryBuilder
            {
                { "token", token },
                { "email", email }
            }.ToString()
        };

        var verificationLink = uri.ToString();
        var subject = Storage.VerifyEmailMailSubject;
        var htmlBody = string.Format(Storage.VerifyEmailMail, verificationLink);
        var textBody = string.Format(Storage.VerifyEmailMailPlain, verificationLink);

        await SendEmailAsync(email, subject, htmlBody, textBody);
    }

    public async Task SendPasswordResetEmailAsync(string email, string token)
    {
        UriBuilder uri = new(_brevoOptions.Value.ResetPasswordRedirect)
        {
            Query = new QueryBuilder
            {
                { "token", token },
                { "email", email }
            }.ToString()
        };

        var resetLink = uri.ToString();
        var subject = Storage.ResetPasswordMailSubject;
        var htmlBody = string.Format(Storage.ResetPasswordMail, resetLink);
        var textBody = string.Format(Storage.ResetPasswordMailPlain, resetLink);

        await SendEmailAsync(email, subject, htmlBody, textBody);
    }

    private async Task SendEmailAsync(string recipientEmail, string subject, string htmlContent, string textContent)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("api-key", _brevoOptions.Value.ApiKey);

        var payload = new
        {
            sender = new { name = _brevoOptions.Value.SenderName, email = _brevoOptions.Value.SenderEmail },
            to = new[] { new { email = recipientEmail } },
            subject,
            htmlContent,
            textContent
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(_brevoOptions.Value.SmtpApi, content);

        response.EnsureSuccessStatusCode();
    }
}