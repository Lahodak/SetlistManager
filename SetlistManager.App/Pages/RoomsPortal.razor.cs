using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Pages.Dialogs;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class RoomsPortal
{
    [Inject]
    public required IDialogService DialogService { get; set; }
    [Inject]
    public required IRoomService RoomService { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    private readonly PagedRequest pageStatus = new();

    private MudTable<RoomModel?> table = new();
    private string? searchString;    

    private async Task<TableData<RoomModel?>> ServerReload(TableState state, CancellationToken token)
    {        
        pageStatus.PageIndex = state.Page;
        pageStatus.PageSize = state.PageSize;
        pageStatus.Query = searchString;        

        var response = await RoomService.GetPublicActiveRoomsAsync(pageStatus);

        if(response?.Items is null)
        {
            return new TableData<RoomModel?>
            {
                TotalItems = 0,
                Items = []
            };
        }

        IEnumerable<RoomModel> filtered = response.Items;

        filtered = state.SortLabel switch
        {
            "name_field" => filtered.OrderByDirection(state.SortDirection, r => r.Name),
            _ => filtered
        };

        return new TableData<RoomModel?>
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

    private async Task CreateRoomClicked()
    {
        var options = new DialogOptions { CloseButton = true };
        var dialog = await DialogService.ShowAsync<CreateRoomDialog>("Create New Room", options);

        var result = await dialog.Result;
        if (!result!.Canceled)
        {
            await table.ReloadServerData();
        }
    }

    private async Task OpenJoinRoomDialog()
    {
        var options = new DialogOptions { CloseButton = true };
        await DialogService.ShowAsync<JoinRoomDialog>("Join Room", options);
    }

    private void JoinSelectedRoom(RoomModel room)
    {
        NavigationManager.NavigateTo($"/room/{room.Code}");
    }    
}