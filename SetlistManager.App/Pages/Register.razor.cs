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
    private readonly RegisterRequestModel _registerRequestModel = new();
    private string confirmPassword = string.Empty;

    private async Task LoginUser()
    {
        if (_registerRequestModel.Password == string.Empty
            || _registerRequestModel.Email == string.Empty
            || _registerRequestModel.UserName == string.Empty
            || confirmPassword != _registerRequestModel.Password)
            return;

        await UserService.RegisterAsync(_registerRequestModel);
        await LocalStorage.SetItemAsStringAsync(_emailKey, _registerRequestModel.Email);

        Navigation.NavigateTo("/verify-email");
    }
}