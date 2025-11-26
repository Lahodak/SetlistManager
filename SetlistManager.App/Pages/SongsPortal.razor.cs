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

    private MudTable<SongModel> table = new();
    private string? searchString;

    private async Task<TableData<SongModel>?> ServerReload(TableState state, CancellationToken token)
    {
        var allSongs = await SongService.GetAllSongsAsync();
        await Task.Delay(300, token);

        if (allSongs is null)
            return null;

        var filtered = allSongs.Where(song =>
            string.IsNullOrWhiteSpace(searchString)
            || song.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
        );

        filtered = state.SortLabel switch
        {
            "title_field" => filtered.OrderByDirection(state.SortDirection, s => s.Name),
            "artist_field" => filtered.OrderByDirection(state.SortDirection, s => s.Artist),
            _ => filtered
        };

        var items = filtered.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

        return new TableData<SongModel>
        {
            TotalItems = filtered.Count(),
            Items = items
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