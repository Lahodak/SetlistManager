using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;

namespace SetlistManager.App.Pages;

public partial class BlankPage
{
    [Inject]
    public required NavigationManager Navigation { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (await UserService.IsUserLoggedInAsync())
        {
            Navigation.NavigateTo("/home");
            return;
        }
        Navigation.NavigateTo("/login");
    }
}