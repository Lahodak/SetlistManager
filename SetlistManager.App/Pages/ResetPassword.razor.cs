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

    private bool _isSubmitting = false;
    private bool _canReset = false;
    private bool _showPassword = false;
    private string? _token;
    private string? _email;
    private string? _newPassword;
    private string? _confirmPassword;
    private string? _errorMessage;
    private const string _tokenParameterKey = "token";
    private const string _emailParameterKey = "email";
    private const string _loginUri = "/login";

    protected override void OnInitialized()
    {
        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var queryParams = QueryHelpers.ParseQuery(uri.Query);

        if (queryParams.TryGetValue(_tokenParameterKey, out var t) && queryParams.TryGetValue(_emailParameterKey, out var e))
        {
            _token = t;
            _email = e;
            _canReset = true;
        }
        else
        {
            _errorMessage = "Invalid reset password link.";
        }
    }

    private async Task HandleResetPassword()
    {
        if (string.IsNullOrEmpty(_newPassword) || string.IsNullOrEmpty(_confirmPassword) || _newPassword != _confirmPassword)
        {
            _errorMessage = "Passwords do not match.";
            return;
        }

        if (_token is null || _email is null)
        {
            _errorMessage = "Invalid reset request.";
            return;
        }

        _isSubmitting = true;

        var result = await UserService.ResetPasswordAsync(_email, _newPassword, _token);

        _isSubmitting = false;

        if (result)
        {
            Snackbar.Add("Password reset successfully! Redirecting to login...", Severity.Success);
            _errorMessage = null;
            NavigationManager.NavigateTo(_loginUri);
        }
        else
        {
            _errorMessage = "Password reset failed. Invalid or expired token.";
        }
    }
}