using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using SetlistManager.Business.Options;
using SetlistManager.Resources.Storage;
using System.Net.Http.Json;

namespace SetlistManager.Business.Services.Implementations;

public class MailService : IMailService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly BrevoOptions _brevoOptions;

    private const string _brevoApiKeySectionName = "api-key";
    private const string _tokenSectionName = "token";
    private const string _emailSectionName = "email";

    public MailService(IHttpClientFactory httpClientFactory, IOptions<BrevoOptions> brevoOptions)
    {
        _httpClientFactory = httpClientFactory;
        _brevoOptions = brevoOptions.Value;
    }

    public async Task SendVerificationEmailAsync(string email, string token)
    {
        UriBuilder uri = new(_brevoOptions.VerifyEmailRedirect)
        {
            Query = new QueryBuilder
            {
                { _tokenSectionName, token },
                { _emailSectionName, email }
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
        UriBuilder uri = new(_brevoOptions.ResetPasswordRedirect)
        {
            Query = new QueryBuilder
            {
                { _tokenSectionName, token },
                { _emailSectionName, email }
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
        client.DefaultRequestHeaders.Add(_brevoApiKeySectionName, _brevoOptions.ApiKey);

        var payload = new
        {
            sender = new { name = _brevoOptions.SenderName, email = _brevoOptions.SenderEmail },
            to = new[] { new { email = recipientEmail } },
            subject,
            htmlContent,
            textContent
        };

        var response = await client.PostAsJsonAsync(_brevoOptions.SmtpApi, payload);
        response.EnsureSuccessStatusCode();
    }
}