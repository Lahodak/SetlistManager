using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using SetlistManager.App.Services;

namespace SetlistManager.App.Pages;

public partial class VerifyEmail
{
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private const string _loginUri = "/login";
    private const string _tokenKey = "token";
    private const string _emailKey = "email";
    private bool isVerifying = false;
    private string? successMessage;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryParams = QueryHelpers.ParseQuery(uri.Query);

        if (queryParams.TryGetValue(_tokenKey, out var token) && queryParams.TryGetValue(_emailKey, out var email))
        {
            isVerifying = true;
            var result = await UserService.VerifyEmailAsync(token!, email!);

            isVerifying = false;
            if (result)
            {
                successMessage = "Email verified successfully! Redirecting to login...";
                Snackbar.Add("Email verified successfully!", Severity.Success);
                NavigationManager.NavigateTo(_loginUri);
            }
            else
            {
                errorMessage = "Verification failed. Invalid or expired token.";
            }
        }
        else
        {
            errorMessage = "Invalid verification link.";
        }
    }
}