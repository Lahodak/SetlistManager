using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Pages.Dialogs;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class ArtistsPortal
{
    [Inject]
    public required IDialogService DialogService { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }
    [Inject]
    public required IArtistService ArtistService { get; set; }

    private MudTable<ArtistModel> _table = new();
    private string? searchString;

    private async Task<TableData<ArtistModel>> ServerReload(TableState state, CancellationToken token)
    {
        var allArtists = await ArtistService.GetAvailableArtistsAsync();
        await Task.Delay(300, token);

        var filtered = allArtists!.Where(artist =>
            string.IsNullOrWhiteSpace(searchString)
            || artist.Nick.Contains(searchString, StringComparison.OrdinalIgnoreCase)
        );

        filtered = state.SortLabel switch
        {
            "nick_field" => filtered.OrderByDirection(state.SortDirection, a => a.Nick),
            "songs_field" => filtered.OrderByDirection(state.SortDirection, a => a.Songs?.Count ?? 0),
            _ => filtered
        };

        var items = filtered.Skip(state.Page * state.PageSize).Take(state.PageSize).ToArray();

        return new TableData<ArtistModel>
        {
            TotalItems = filtered.Count(),
            Items = items
        };
    }

    private async Task DeleteArtistAsync(ArtistModel artist)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Confirm Delete",
            $"Are you sure you want to delete the artist '{artist.Nick}' along with it's songs?",
            yesText: "Delete", noText: "Cancel", options: new DialogOptions { CloseOnEscapeKey = true }
        );

        if (result == true)
        {
            var deleteResult = await ArtistService.TryDeleteArtistAsync(artist.Id);
            if (deleteResult)
            {
                Snackbar.Add("Artist deleted successfully!", Severity.Success);
                await _table.ReloadServerData();
            }
            else
            {
                Snackbar.Add("Failed to delete artist.", Severity.Error);
            }
        }
    }

    private async Task UpdateArtistAsync(ArtistModel artist)
    {
        var parameters = new DialogParameters { ["ArtistToEdit"] = artist };
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };
        var dialog = await DialogService.ShowAsync<EditArtistDialog>("Edit Artist", parameters, options);
        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            Snackbar.Add("Artist updated successfully!", Severity.Success);
            await _table.ReloadServerData();
        }
    }

    private void OnSearch(string text)
    {
        searchString = text;
        _table.ReloadServerData();
    }

    private async Task OpenCreateArtistDialog()
    {
        var parameters = new DialogParameters();
        var options = new DialogOptions { CloseOnEscapeKey = true, MaxWidth = MaxWidth.Medium, FullWidth = true };

        var dialog = await DialogService.ShowAsync<CreateNewArtistDialog>("Create New Artist", parameters, options);
        var result = await dialog.Result;

        if (!result!.Canceled)
        {
            Snackbar.Add("Artist created successfully!", Severity.Success);
            await _table.ReloadServerData();
        }
    }
}