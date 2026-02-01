using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using SetlistManager.App.Services;

namespace SetlistManager.App.Pages;

public partial class ResetPassword
{
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private const string _tokenParameterKey = "token";
    private const string _emailParameterKey = "email";
    private const string _loginUri = "/login";
    private bool isSubmitting = false;
    private bool canReset = false;

    private string? token;
    private string? email;
    private string? newPassword;
    private string? confirmPassword;
    private string? errorMessage;

    protected override void OnInitialized()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryParams = QueryHelpers.ParseQuery(uri.Query);

        if (queryParams.TryGetValue(_tokenParameterKey, out var t) && queryParams.TryGetValue(_emailParameterKey, out var e))
        {
            token = t;
            email = e;
            canReset = true;
        }
        else
        {
            errorMessage = "Invalid reset password link.";
        }
    }

    private async Task HandleResetPassword()
    {
        if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword) || newPassword != confirmPassword)
        {
            errorMessage = "Passwords do not match.";
            return;
        }

        if (token is null || email is null)
        {
            errorMessage = "Invalid reset request.";
            return;
        }

        isSubmitting = true;

        var result = await UserService.ResetPasswordAsync(email, newPassword, token);

        isSubmitting = false;

        if (result)
        {
            Snackbar.Add("Password reset successfully! Redirecting to login...", Severity.Success);
            errorMessage = null;
            NavigationManager.NavigateTo(_loginUri);
        }
        else
        {
            errorMessage = "Password reset failed. Invalid or expired token.";
        }
    }
}