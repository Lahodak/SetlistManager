using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Models;
using SetlistManager.App.Pages.Dialogs;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;
using System.Reflection;

namespace SetlistManager.App.Pages;

public partial class SetlistsPortal
{
    [Inject]
    public required ISetlistService SetlistService { get; set; }
    [Inject]
    public required IDialogService DialogService { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }

    private MudTable<SetlistModel> table = new();
    private PagedRequest pageStatus = new() { ContentType = ContentType.Private };
    private string? searchString;
    private int _userId;

    protected override async Task OnInitializedAsync()
    {
        _userId = (await UserService.GetCurrentUserIdAsync()).Value;
    }

    private async Task<TableData<SetlistModel>?> ServerReload(TableState state, CancellationToken token)
    {
        await Task.Delay(300, token);
        pageStatus.Query = searchString;
        pageStatus.PageIndex = state.Page;
        pageStatus.PageSize = state.PageSize;
        

        var response = await SetlistService.GetAllSetlistsAsync(pageStatus);

        if (response?.Items is null)
        {
            return new TableData<SetlistModel>
            {
                TotalItems = 0,
                Items = []
            };
        }        

        var filtered = response.Items.AsQueryable();

        filtered = state.SortLabel switch
        {
            "name_field" => filtered.OrderByDirection(state.SortDirection, s => s.Name),
            _ => filtered
        };


        return new TableData<SetlistModel>
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

    private async Task EditSetlist(int id)
    {
        var existing = await SetlistService.GetSetlistById(id);

        if (existing is null)
        {
            Snackbar.Add("Failed editing setlist", Severity.Error);
            return;
        }

        var parameters = new DialogParameters { ["Setlist"] = existing };
        var options = new DialogOptions { CloseButton = true };
        var dialog = await DialogService.ShowAsync<EditSetlistDialog>($"Edit Setlist", parameters, options);

        var result = await dialog.Result;

        if (result is null || !result.Canceled)
        {
            await table.ReloadServerData();
        }
    }

    private async Task DeleteSetlist(SetlistModel model)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Confirm Delete",
            $"Are you sure you want to delete the setlist '{model.Name}'?",
            yesText: "Delete", noText: "Cancel", options: new DialogOptions { CloseOnEscapeKey = true }
        );

        if (result is not true)
            return;

        var success = await SetlistService.TryDeleteSetlistAsync(model.Id);

        if (!success)
        {
            Snackbar.Add("Failed deleting setlist", Severity.Error);
            return;
        }

        Snackbar.Add("Setlist deleted successfully!", Severity.Success);

        await table.ReloadServerData();
    }

    private async Task RemoveSetlistFromUserLibrary(SetlistModel model)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Confirm Removal",
            $"Are you sure you want to reomve the setlist '{model.Name}' from your Library?",
            yesText: "Remove", noText: "Cancel", options: new DialogOptions { CloseOnEscapeKey = true }
        );

        if (result is not true)
            return;

        await SetlistService.RemoveAccessFromUserAsync(model.Id, _userId);
        await table.ReloadServerData();
    }

    private async Task OpenGenerateDialog()
    {
        var options = new DialogOptions { CloseButton = true };
        var dialog = await DialogService.ShowAsync<CreateSetlistDialog>("", options);

        var result = await dialog.Result;
        if (result is null || !result.Canceled)
        {
            await table.ReloadServerData();
        }
    }

    public async Task AddAccessToUserAsync(int id)
    {
        var parameters = new DialogParameters
        {
            { "ContentType", ShareContentType.Setlist },
            { "ContentId", id }
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        await DialogService.ShowAsync<ShareContentDialog>("Share Setlist", parameters, options);
    }
}