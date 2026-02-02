using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Models;
using SetlistManager.App.Pages.Dialogs;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class SongsPortal
{
    [Inject]
    public required IDialogService DialogService { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }
    [Inject]
    public required ISongService SongService { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }

    private MudTable<SongModel> table = new();
    private readonly PagedRequest _pageState = new() { ContentType = ContentType.Private };
    private string? searchString;
    private int _userId;

    protected override async Task OnInitializedAsync()
    {
        _userId = (await UserService.GetCurrentUserIdAsync()).Value;
    }

    private async Task<TableData<SongModel>?> ServerReload(TableState state, CancellationToken token)
    {
        _pageState.Query = searchString;
        _pageState.PageIndex = state.Page;
        _pageState.PageSize = state.PageSize;

        var response = await SongService.GetAllSongsAsync(_pageState);

        if (response?.Items is null)
            return null;

        IEnumerable<SongModel>? filtered = response.Items;

        filtered = state.SortLabel switch
        {
            "title_field" => filtered.OrderByDirection(state.SortDirection, s => s.Name),
            "artist_field" => filtered.OrderByDirection(state.SortDirection, s => s.Artist),
            _ => filtered
        };

        return new TableData<SongModel>
        {
            TotalItems = response.TotalCount,
            Items = filtered
        };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }

    private async Task OpenAddSongDialog()
    {
        var parameters = new DialogParameters();
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };

        var dialog = await DialogService.ShowAsync<CreateSongDialog>("Create New Song", parameters, options);
        var result = await dialog.Result;

        if (!result!.Canceled)
        {
            Snackbar.Add("Song added successfully!", Severity.Success);
            await table.ReloadServerData();
        }
    }

    private async Task OpenEditSongDialog(SongModel song)
    {
        var parameters = new DialogParameters { { "Song", song } };
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };

        var dialog = await DialogService.ShowAsync<EditSongDialog>("Update Song", parameters, options);
        var result = await dialog.Result;

        if (!result!.Canceled)
        {
            Snackbar.Add("Song updated successfully!", Severity.Success);
            await table.ReloadServerData();
        }
    }

    private async Task DeleteSongAsync(SongModel song)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Confirm Delete",
            $"Are you sure you want to delete the song '{song.Name}'?",
            yesText: "Delete", noText: "Cancel", options: new DialogOptions { CloseOnEscapeKey = true }
        );

        if (result is not true)
            return;

        bool deleteResult = await SongService.TryDeleteSongAsync(song.Id);

        if (!deleteResult)
        {
            Snackbar.Add("Failed to delete song.", Severity.Error);
            return;
        }
        
        Snackbar.Add("Song deleted successfully!", Severity.Success);
        await table.ReloadServerData();        
    }

    private async Task RemoveSongFromUserLibraryAsync(SongModel song)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Confirm Removal",
            $"Are you sure you want to remove the song '{song.Name}' from your Library?",
            yesText: "Remove", noText: "Cancel", options: new DialogOptions { CloseOnEscapeKey = true }
        );

        if (result is not true)
            return;

        await SongService.RemoveAccessFromUserAsync(song.Id, _userId);
        await table.ReloadServerData();
    }

    public async Task MakeSongPublicAsync(SongModel song)
    {
        bool? dialogResult = await DialogService.ShowMessageBox(
            "Confirm Publishing",
            $"Are you sure you want to make the song '{song.Name}' public? This action cannot be undone.",
            yesText: "Make Public", noText: "Cancel", options: new DialogOptions { CloseOnEscapeKey = true }
        );

        if (dialogResult is not true)
            return;

        await SongService.TryMakeSongPublicAsync(song.Id);
        Snackbar.Add($"{song.Name} is now public!", Severity.Success);
        
        await table.ReloadServerData();
    }

    public async Task AddAccessToUserAsync(int id)
    {
        var parameters = new DialogParameters
        {
            { "ContentType", ShareContentType.Song },
            { "ContentId", id }
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        await DialogService.ShowAsync<ShareContentDialog>("Share Song", parameters, options);
    }
}