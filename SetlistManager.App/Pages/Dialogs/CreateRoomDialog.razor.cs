using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages.Dialogs;

public partial class CreateRoomDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }
    [Inject]
    public required IRoomService RoomService { get; set; }
    [Inject]
    public required ISetlistService SetlistService { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private readonly RoomCreateModel _createRoomModel = new();
    private List<SetlistModel>? _availableSetlists;
    private SetlistModel? _selectedSetlist;
    private string _searchString = string.Empty;
    private readonly HashSet<SetlistModel> _expandedSetlists = [];

    protected override async Task OnInitializedAsync()
    {
        _availableSetlists = (await SetlistService.GetAllSetlistsAsync(new() { PageSize = int.MaxValue }))?.Items;

        if (_availableSetlists is null)
        {
            Snackbar.Add("Couldn't find any available Setlists", Severity.Error);
        }
    }

    private void SelectSetlist(SetlistModel setlist)
    {
        _selectedSetlist = setlist;
    }

    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(_createRoomModel.Name))
            return;

        _createRoomModel.SetlistModel = _selectedSetlist;

        RoomModel? createdRoom = await RoomService.CreateRoomAsync(_createRoomModel);

        if (createdRoom is not null && createdRoom.Code is not null)
            NavigationManager.NavigateTo($"/room/{createdRoom.Code}");

        MudDialog.Close(DialogResult.Ok(createdRoom));
    }

    private bool FilterSetlists(SetlistModel setlist)
    {
        if (string.IsNullOrWhiteSpace(_searchString))
            return true;

        if (setlist.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private void ToggleRowExpand(SetlistModel setlist)
    {
        if (!_expandedSetlists.Remove(setlist))
        {
            _expandedSetlists.Add(setlist);
        }
    }

    private bool IsRowExpanded(SetlistModel setlist) => _expandedSetlists.Contains(setlist);

    private void Cancel() => MudDialog.Cancel();
}