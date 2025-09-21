using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SetlistManager.Business.Services;

public class MailService : IMailService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    private readonly string _apiKey;
    private readonly string _senderEmail;
    private readonly string _senderName;

    public MailService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;

        _apiKey = _configuration["Brevo:ApiKey"]!;
        _senderEmail = _configuration["Brevo:SenderEmail"]!;
        _senderName = _configuration["Brevo:SenderName"]!;
    }

    public async Task SendVerificationEmailAsync(string email, string token)
    {
        var subject = "Verify your Setlist Manager account";
        var body = $"<p>Your verification code is: <b>{token}</b></p>";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string email, string token)
    {
        var subject = "Reset your Setlist Manager password";
        var body = $"<p>Use the following code to reset your password: <b>{token}</b></p>";

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string recipientEmail, string subject, string htmlContent)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);

        var payload = new
        {
            sender = new { email = _senderEmail, name = _senderName },
            to = new[] { new { email = recipientEmail } },
            subject = subject,
            htmlContent = htmlContent
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync("https://api.brevo.com/v3/smtp/email", content);
        response.EnsureSuccessStatusCode();
    }
}