using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;

namespace SetlistManager.App.Pages;

public partial class BlankPage
{
    [Inject]
    public required NavigationManager Navigation { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }
    private const string _homeUri = "/home";
    private const string _loginUri = "/login";

    protected override async Task OnInitializedAsync()
    {
        if (await UserService.VerifyStoredToken())
        {
            Navigation.NavigateTo(_homeUri);
            return;
        }
        Navigation.NavigateTo(_loginUri);
    }
}