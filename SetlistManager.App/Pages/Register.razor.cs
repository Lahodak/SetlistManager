using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class Register
{
    [Inject]
    public required IUserService UserService { get; set; }
    [Inject]
    public required NavigationManager Navigation { get; set; }
    [Inject]
    public required ILocalStorageService LocalStorage { get; set; }

    private const string _emailKey = "registeredEmail";
    private const string _verifyEmailUri = "/verify-email";
    private readonly RegisterRequestModel _registerRequestModel = new();
    private string confirmPassword = string.Empty;

    private async Task LoginUser()
    {
        if (string.IsNullOrEmpty(_registerRequestModel.Password)
            || string.IsNullOrEmpty(_registerRequestModel.Email)
            || string.IsNullOrEmpty(_registerRequestModel.UserName)
            || confirmPassword != _registerRequestModel.Password)
            return;

        await UserService.RegisterAsync(_registerRequestModel);
        await LocalStorage.SetItemAsStringAsync(_emailKey, _registerRequestModel.Email);

        Navigation.NavigateTo(_verifyEmailUri);
    }
}