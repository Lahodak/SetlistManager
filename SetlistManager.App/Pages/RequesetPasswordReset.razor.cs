using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;

namespace SetlistManager.App.Pages;

public partial class RequesetPasswordReset
{
    [Inject]
    public required IUserService UserService { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    private string email = string.Empty;
    private string? successMessage;
    private string? errorMessage;
    private bool isSubmitting = false;

    private async Task HandlePasswordResetRequest()
    {

        if (string.IsNullOrWhiteSpace(email))
        {
            errorMessage = "Please enter your email.";
            return;
        }

        isSubmitting = true;
        var result = await UserService.RequestPasswordResetAsync(email);
        isSubmitting = false;

        if (result)
        {
            successMessage = "If an account with this email exists, a reset link has been sent.";
            errorMessage = null;
        }
        else
        {
            errorMessage = "Failed to send reset email. Please try again.";
        }
    }
}