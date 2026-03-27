using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class UserPortal
{
    [Inject]
    public required IUserService UserService { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IInstrumentService InstrumentService { get; set; }
    [Inject]
    public required IGeniusService GeniusService { get; set; }

    private UserModel? _userModel;
    private List<InstrumentModel>? _instruments;
    private const string _loginUri = "/login";

    protected override async Task OnInitializedAsync()
    {
        _userModel = await UserService.GetUserAsync();
        _instruments = await InstrumentService.GetAvailableInstrumentsAsync();

        if (_userModel is not null && _instruments is not null && _userModel.Instrument is not null)
        {
            _userModel.Instrument = _instruments.FirstOrDefault(i => i.Id == _userModel.Instrument.Id);
        }
    }

    private async Task SaveAsync()
    {
        if (_userModel is not null)
        {
            await UserService.TryUpdateUser(_userModel);
            NavigationManager.Refresh(true);
        }
    }

    private async Task LogOutAsync()
    {
        await UserService.LogOutAsync();
        NavigationManager.NavigateTo(_loginUri);
    }

    private async Task AuthorizeWithGenius()
    {
        NavigationManager.NavigateTo(await GeniusService.AuthorizeAsync());
    }

    private async Task RevokeGeniusTokenAsync()
    {
        var geniusToken = _userModel?.Tokens?.FirstOrDefault(t => t.Provider == ProviderEnum.Genius.ToString());

        if (geniusToken is not null && await UserService.TryRevokeTokenAsync(geniusToken.Id))
        {
            _userModel!.Tokens!.Remove(geniusToken);
        }
    }
}