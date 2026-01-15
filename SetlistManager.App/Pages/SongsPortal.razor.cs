using Microsoft.AspNetCore.Components;
using MudBlazor;
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
    private PagedRequest pageState = new() { ContentType = ContentType.Private };
    private string? searchString;
    private int _userId;

    protected override async Task OnInitializedAsync()
    {
        _userId = (await UserService.GetCurrentUserIdAsync()).Value;
    }

    private async Task<TableData<SongModel>?> ServerReload(TableState state, CancellationToken token)
    {
        await Task.Delay(300, token);

        pageState.Query = searchString;
        pageState.PageIndex = state.Page;
        pageState.PageSize = state.PageSize;

        var response = await SongService.GetAllSongsAsync(pageState);

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
}