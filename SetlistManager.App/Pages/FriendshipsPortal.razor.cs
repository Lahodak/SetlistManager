using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Pages.Dialogs;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class FriendshipsPortal
{
    [Inject]
    public required IUserService UserService { get; set; }    
    [Inject]
    public required IDialogService DialogService { get; set; }    
    [Inject]
    public required ISnackbar Snackbar { get; set; }
    
    private MudTable<FriendModel> _table = new();
    private PagedRequest pageStatus = new();
    private string? searchString;

    private async Task<TableData<FriendModel>> ServerReload(TableState state, CancellationToken token)
    {
        await Task.Delay(300, token);
        pageStatus.PageIndex = state.Page;
        pageStatus.PageSize = state.PageSize;
        pageStatus.Query = searchString;
        var result = await UserService.GetUserFriendshipsAsync(pageStatus);
        if (result?.Items is null)
        {
            return new TableData<FriendModel>
            {
                TotalItems = 0,
                Items = []
            };
        }
        var filtered = result.Items.AsQueryable();
        filtered = state.SortLabel switch
        {
            "username_field" => filtered.OrderByDirection(state.SortDirection, f => f.Username),
            "state_field" => filtered.OrderByDirection(state.SortDirection, f => f.State),
            _ => filtered
        };
        return new TableData<FriendModel>
        {
            TotalItems = result.TotalCount,
            Items = filtered
        };
    }

    private async Task AcceptFriendshipAsync(FriendModel friend)
    {
        var acceptResult = await UserService.TryAcceptFriendshipAsync(friend.FriendshipId);
        if (acceptResult)
        {
            Snackbar.Add($"You are now friends with {friend.Username}!", Severity.Success);
            await _table.ReloadServerData();
        }
        else
        {
            Snackbar.Add("Failed to accept friendship.", Severity.Error);
        }
    }

    private async Task RemoveFriendshipAsync(FriendModel friend)
    {
        bool? result = await DialogService.ShowMessageBox(
            "Confirm Remove",
            $"Are you sure you want to remove '{friend.Username}' from your friends?",
            yesText: "Remove",
            noText: "Cancel",
            options: new DialogOptions { CloseOnEscapeKey = true }
        );
        
        if (result is not true)
            return;

        var deleteResult = await UserService.TryRemoveFriendshipAsync(friend.FriendshipId);

        if (deleteResult)
        {
            Snackbar.Add("Friendship removed successfully!", Severity.Success);
            await _table.ReloadServerData();
        }
        else
        {
            Snackbar.Add("Failed to remove friendship.", Severity.Error);
        }
    }

    private void OnSearch(string text)
    {
        searchString = text;
        _table.ReloadServerData();
    }

    private async Task OpenAddFriendDialog()
    {
        var options = new DialogOptions 
        { 
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };
        
        var dialog = await DialogService.ShowAsync<CreateFriendshipDialog>("Add Friend", options);
        var result = await dialog.Result;
        
        if (!result!.Canceled)
        {
            await _table.ReloadServerData();
        }
    }
}