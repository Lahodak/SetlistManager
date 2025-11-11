using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Pages.Dialogs;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class Room
{
    [Parameter]
    public string RoomCode { get; set; } = string.Empty;
    [Inject]
    public required IRoomService RoomService { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }
    [Inject]
    public required IDialogService DialogService { get; set; }

    private RoomModel? _roomModel;
    private SongModel? _currentSong;
    private SongModel? _prevSong;
    private SongModel? _nextSong;
    private UserModel? _user;

    private int _currentIndex = 1;

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(RoomCode))
        {
            NavigationManager.NavigateTo("/error");
            return;
        }

        JoinRoomModel joinRoomModel = new()
        {
            RoomCode = RoomCode
        };

        RoomService.RoomUpdated += OnRoomUpdated;

        _roomModel = await RoomService.JoinRoomAsync(joinRoomModel);

        if (_roomModel is null)
        {
            NavigationManager.NavigateTo("/roomnotfound");
            return;
        }

        if (_roomModel.Setlist is null)
            return;

        _currentSong = _roomModel!.Setlist?.Songs.FirstOrDefault(x => x.Id == _roomModel.CurrentSong);
        _roomModel.CurrentSong = _currentSong?.Id;

        if (_currentSong is not null)
            _currentIndex = _currentSong.Order;

        if (_currentSong is null)
            return;

        _currentIndex = _currentSong.Order;

        if (_currentIndex > 1)
        {
            _prevSong = _roomModel.Setlist!.Songs.First(x => x.Order == _currentIndex - 1);
        }

        if (_currentIndex < _roomModel.Setlist!.Songs.Count)
        {
            _nextSong = _roomModel.Setlist.Songs.First(x => x.Order == _currentIndex + 1);
        }

        _user = await UserService.GetUserAsync();
        StateHasChanged();
    }

    private async Task MoveToNextSong()
    {
        await UpdateCurrentSongAsync(1);
        StateHasChanged();
    }

    private void OnRoomUpdated(RoomModel room)
    {
        _roomModel = room;

        if (_roomModel?.Setlist is null || _roomModel.CurrentSong is null)
            return;

        _currentSong = _roomModel.Setlist.Songs.First(x => x.Id == _roomModel.CurrentSong);
        _currentIndex = _currentSong.Order;

        _prevSong = _currentIndex > 1
            ? _roomModel.Setlist.Songs.First(x => x.Order == _currentIndex - 1)
            : null;

        _nextSong = _currentIndex < _roomModel.Setlist.Songs.Count
            ? _roomModel.Setlist.Songs.First(x => x.Order == _currentIndex + 1)
            : null;

        StateHasChanged();
    }

    private async Task MoveToPrevSong()
    {
        await UpdateCurrentSongAsync(-1);
        StateHasChanged();
    }

    private async Task UpdateCurrentSongAsync(int operation)
    {
        if (_roomModel is null || _roomModel.Setlist is null || _currentIndex + operation > _roomModel.Setlist.Songs.Count || _currentIndex + operation < 1)
            return;

        ChangeCurrentSongModel changeCurrentSongModel = new()
        {
            RoomId = _roomModel.Id,
            AdminId = _roomModel.HostId,
            CurrentSongId = _roomModel.CurrentSong!.Value,
            NewCurrentSongId = _roomModel.Setlist.Songs.First(x => x.Order == _currentIndex + operation).Id
        };

        var room = await RoomService.ChangeCurrentSongAsync(changeCurrentSongModel);

        if (room is null)
            return;

        OnRoomUpdated(room);
    }

    private async Task OpenSetlistContentDialog()
    {
        if (_roomModel is null || _roomModel.Setlist is null || _currentSong is null)
            return;

        var parameters = new DialogParameters<ShowSetlistContentDialog>
        {
            { nameof(ShowSetlistContentDialog.Setlist), _roomModel.Setlist },
            { nameof(ShowSetlistContentDialog.CurrentSongId), _currentSong.Id }
        };

        var options = new DialogOptions { CloseButton = true };

        await DialogService.ShowAsync<ShowSetlistContentDialog>("Setlist Content", parameters, options);
    }
}