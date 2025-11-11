using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using SetlistManager.App.Services;

namespace SetlistManager.App.Pages;

public partial class ResetPassword
{
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }

    private bool isSubmitting = false;
    private bool canReset = false;

    private string? token;
    private string? email;
    private string? newPassword;
    private string? confirmPassword;
    private string? successMessage;
    private string? errorMessage;

    protected override void OnInitialized()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryParams = QueryHelpers.ParseQuery(uri.Query);

        if (queryParams.TryGetValue("token", out var t) && queryParams.TryGetValue("email", out var e))
        {
            token = t!;
            email = e!;
            canReset = true;
        }
        else
        {
            errorMessage = "Invalid reset password link.";
        }
    }

    private async Task HandleResetPassword()
    {
        if (newPassword is null || confirmPassword is null || newPassword != confirmPassword)
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
            successMessage = "Password reset successfully! Redirecting to login...";
            errorMessage = null;
            await Task.Delay(2000);
            NavigationManager.NavigateTo("/login");
        }
        else
        {
            errorMessage = "Password reset failed. Invalid or expired token.";
        }
    }
}