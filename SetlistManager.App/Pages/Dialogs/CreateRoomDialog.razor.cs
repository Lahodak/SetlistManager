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
    public required ISnackbar Snackbar { get; set; }

    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    private readonly RoomCreateModel _createRoomModel = new();
    private List<SetlistModel>? _availableSetlists;
    private SetlistModel? _selectedSetlist;
    private readonly HashSet<SetlistModel> _expandedSetlists = [];
    private string _searchString = string.Empty;
    private bool _isLoading;

    protected override async Task OnInitializedAsync()
    {
        await LoadSetlists();
    }

    private async Task LoadSetlists()
    {
        _isLoading = true;

        var result = await SetlistService.GetSetlistsAsync(new()
        {
            PageSize = 10,
            Query = _searchString
        });

        _availableSetlists = result?.Items;

        if (_availableSetlists is null || !_availableSetlists.Any())
        {
            Snackbar.Add("Couldn't find any available Setlists", Severity.Info);
        }

        _isLoading = false;
    }

    private async Task OnSearchChanged(string searchValue)
    {
        _searchString = searchValue;
        await LoadSetlists();
    }

    private void SelectSetlist(SetlistModel setlist)
    {
        _selectedSetlist = setlist;
    }

    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(_createRoomModel.Name))
        {
            Snackbar.Add("Please enter a room name", Severity.Warning);
            return;
        }

        if (_createRoomModel.Name.Length < 3)
        {
            Snackbar.Add("Room name must be at least 3 characters", Severity.Warning);
            return;
        }

        if (_selectedSetlist is null)
        {
            Snackbar.Add("Please select a setlist", Severity.Warning);
            return;
        }

        _createRoomModel.SetlistModel = _selectedSetlist;

        RoomModel? createdRoom = await RoomService.CreateRoomAsync(_createRoomModel);

        if (createdRoom is not null && createdRoom.Code is not null)
        {
            NavigationManager.NavigateTo($"/room/{createdRoom.Code}");
            MudDialog.Close(DialogResult.Ok(createdRoom));
        }
        else
        {
            Snackbar.Add("Failed to create room", Severity.Error);
        }
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