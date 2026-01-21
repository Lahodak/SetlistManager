using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Models;
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
    [Inject]
    public required IUserService UserService { get; set; }

    private MudTable<ArtistModel> _table = new();
    private PagedRequest pageStatus = new() { ContentType = ContentType.Private };
    private string? searchString;
    private int _userId;

    protected override async Task OnInitializedAsync()
    {
        _userId = (await UserService.GetCurrentUserIdAsync()).Value;
    }

    private async Task<TableData<ArtistModel>> ServerReload(TableState state, CancellationToken token)
    {
        await Task.Delay(300, token);
        pageStatus.PageIndex = state.Page;
        pageStatus.PageSize = state.PageSize;
        pageStatus.Query = searchString;

        var result = await ArtistService.GetAvailableArtistsAsync(pageStatus);

        if(result?.Items is null)
        {
            return new TableData<ArtistModel>
            {
                TotalItems = 0,
                Items = []
            };
        }

        var filtered = result.Items.AsQueryable();

        filtered = state.SortLabel switch
        {
            "nick_field" => filtered.OrderByDirection(state.SortDirection, a => a.Nick),
            "songs_field" => filtered.OrderByDirection(state.SortDirection, a => a.Songs != null ? a.Songs.Count : 0),
            _ => filtered
        };

        return new TableData<ArtistModel>
        {
            TotalItems = result.TotalCount,
            Items = filtered
        };
    }

    private async Task DeleteArtistAsync(ArtistModel artist)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Confirm Delete",
            $"Are you sure you want to delete the artist '{artist.Nick}' along with it's songs?",
            yesText: "Delete", noText: "Cancel", options: new DialogOptions { CloseOnEscapeKey = true }
        );

        if (result is not true)
            return;

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

    private async Task RemoveArtistFromLibraryAsync(ArtistModel artist)
    {
        if(artist.Songs?.Count > 0 )
        {
            Snackbar.Add("You must remove all songs of this artist from your library before removing the artist.", Severity.Warning);
            return;
        }

        bool? result = await DialogService.ShowMessageBox(
            "Confirm Removal",
            $"Are you sure you want to remove the artist '{artist.Nick}' from your library?",
            yesText: "Remove", noText: "Cancel", options: new DialogOptions { CloseOnEscapeKey = true }
        );
        
        if (result is not true)
            return;

        await ArtistService.RemoveAccessFromUserAsync(artist.Id, _userId);
        await _table.ReloadServerData();
    }

    private async Task UpdateArtistAsync(ArtistModel artist)
    {
        var parameters = new DialogParameters { ["ArtistToEdit"] = artist };
        var options = new DialogOptions { CloseOnEscapeKey = true };
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
        var options = new DialogOptions { CloseOnEscapeKey = true };

        var dialog = await DialogService.ShowAsync<CreateArtistDialog>("Create New Artist", parameters, options);
        var result = await dialog.Result;

        if (!result!.Canceled)
        {
            Snackbar.Add("Artist created successfully!", Severity.Success);
            await _table.ReloadServerData();
        }
    }

    public async Task AddAccessToUserAsync(int id)
    {
        var parameters = new DialogParameters
        {
            { "ContentType", ShareContentType.Artist },
            { "ContentId", id }
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        await DialogService.ShowAsync<ShareContentDialog>("Share Artist", parameters, options);
    }
}