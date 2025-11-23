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
                { "token", Uri.EscapeDataString(token) },
                { "email", Uri.EscapeDataString(email) }
            }.ToString()
        };

        var subject = Storage.VerifyEmailMailSubject;
        var body = string.Format(Storage.VerifyEmailMail, uri.ToString());        

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string email, string token)
    {
        UriBuilder uri = new(_brevoOptions.Value.ResetPasswordRedirect)
        {
            Query = new QueryBuilder
            {
                { "token", Uri.EscapeDataString(token) },
                { "email", Uri.EscapeDataString(email) }
            }.ToString()
        };

        var subject = Storage.ResetPasswordMailSubject;
        var body = string.Format(Storage.ResetPasswordMail, uri.ToString());

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