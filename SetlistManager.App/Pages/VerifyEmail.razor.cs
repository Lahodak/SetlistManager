using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using SetlistManager.App.Services;

namespace SetlistManager.App.Pages;

public partial class VerifyEmail
{
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }
    private bool isVerifying = false;
    private string? successMessage;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryParams = QueryHelpers.ParseQuery(uri.Query);

        if (queryParams.TryGetValue("token", out var token) && queryParams.TryGetValue("email", out var email))
        {
            isVerifying = true;
            var result = await UserService.VerifyEmailAsync(token!, email!);

            isVerifying = false;
            if (result)
            {
                successMessage = "Email verified successfully! Redirecting to login...";
                await Task.Delay(2000);
                NavigationManager.NavigateTo("/login");
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