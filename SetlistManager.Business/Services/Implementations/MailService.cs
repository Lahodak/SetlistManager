using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using SetlistManager.Business.Options;
using SetlistManager.Common.Brevo.Models;
using SetlistManager.Common.Exceptions;
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
        var emailTemplate = CreateVerificationEmailTemplate(email, token);
        await SendEmailAsync(email, emailTemplate);
    }

    public async Task SendPasswordResetEmailAsync(string email, string token)
    {
        var emailTemplate = CreatePasswordResetEmailTemplate(email, token);
        await SendEmailAsync(email, emailTemplate);
    }

    private EmailTemplate CreateVerificationEmailTemplate(string email, string token)
    {
        var verificationLink = BuildRedirectUri(_brevoOptions.VerifyEmailRedirect, email, token);

        return new EmailTemplate
        {
            Subject = Storage.VerifyEmailMailSubject,
            HtmlContent = string.Format(Storage.VerifyEmailMail, verificationLink),
            TextContent = string.Format(Storage.VerifyEmailMailPlain, verificationLink)
        };
    }

    private EmailTemplate CreatePasswordResetEmailTemplate(string email, string token)
    {
        var resetLink = BuildRedirectUri(_brevoOptions.ResetPasswordRedirect, email, token);

        return new EmailTemplate
        {
            Subject = Storage.ResetPasswordMailSubject,
            HtmlContent = string.Format(Storage.ResetPasswordMail, resetLink),
            TextContent = string.Format(Storage.ResetPasswordMailPlain, resetLink)
        };
    }

    private string BuildRedirectUri(string baseUri, string email, string token)
    {
        UriBuilder uri = new(baseUri)
        {
            Query = new QueryBuilder
            {
                { _tokenSectionName, token },
                { _emailSectionName, email }
            }.ToString()
        };

        return uri.ToString();
    }

    private async Task SendEmailAsync(string recipientEmail, EmailTemplate template)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add(_brevoApiKeySectionName, _brevoOptions.ApiKey);

        var payload = new
        {
            sender = new { name = _brevoOptions.SenderName, email = _brevoOptions.SenderEmail },
            to = new[] { new { email = recipientEmail } },
            subject = template.Subject,
            htmlContent = template.HtmlContent,
            textContent = template.TextContent
        };

        var response = await client.PostAsJsonAsync(_brevoOptions.SmtpApi, payload);

        if (!response.IsSuccessStatusCode)
            throw new FailedToSendEmailException($"Failed to send email to {recipientEmail}. Status Code: {response.StatusCode}");
    }
}