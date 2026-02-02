using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SetlistManager.App.Pages.Dialogs;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class Room : IAsyncDisposable
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
    [Inject] 
    public required IJSRuntime JSRuntime { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private const string _roomsPortalUri = "/RoomsPortal";
    private const string _toggleFullscreenMethod = "toggleFullscreen";
    private const string _scrollToCurrentSongMethod = "scrollToCurrentSong";
    private RoomModel? _roomModel;
    private SongModel? _currentSong;
    private UserModel? _user;
    private bool _isFullscreen = false;
    private IJSObjectReference? _jsModule;

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(RoomCode))
        {
            Snackbar.Add("RoomCode cannot be blank!", Severity.Error);
            NavigationManager.NavigateTo(_roomsPortalUri);
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
            Snackbar.Add($"Room with the code {RoomCode} not found", Severity.Error);
            NavigationManager.NavigateTo(_roomsPortalUri);
            return;
        }

        if (_roomModel.Setlist is null) return;

        _currentSong = _roomModel.Setlist.Songs.FirstOrDefault(x => x.Id == _roomModel.CurrentSong);
        _roomModel.CurrentSong = _currentSong?.Id;

        _user = await UserService.GetUserAsync();

        StateHasChanged();
        await ScrollToCurrentSong();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            try
            {
                _jsModule = await JSRuntime.InvokeAsync<IJSObjectReference>("import", "/js/fullscreen.js");
            }
            catch
            {
                Snackbar.Add("Fullscreen mode is not supported by your browser.", Severity.Warning);
            }
        }

        if (!firstRender && _currentSong != null)
        {
            await ScrollToCurrentSong();
        }
    }

    private async Task ToggleFullscreen()
    {
        try
        {
            if (_jsModule != null)
            {
                _isFullscreen = await _jsModule.InvokeAsync<bool>(_toggleFullscreenMethod);
                StateHasChanged();
            }
        }
        catch
        {
            Snackbar.Add("Fullscreen mode is not supported by your browser.", Severity.Warning);
        }
    }

    private void OnRoomUpdated(RoomModel room)
    {
        _roomModel = room;

        if (_roomModel?.Setlist is null || _roomModel.CurrentSong is null) 
            return;

        _currentSong = _roomModel.Setlist.Songs.FirstOrDefault(x => x.Id == _roomModel.CurrentSong);

        StateHasChanged();
    }

    private async Task SelectSong(SongModel song)
    {
        if (_roomModel is null || _currentSong is null || song.Id == _currentSong.Id) 
            return;

        ChangeCurrentSongModel changeCurrentSongModel = new()
        {
            RoomId = _roomModel.Id,
            AdminId = _roomModel.HostId,
            CurrentSongId = _currentSong.Id,
            NewCurrentSongId = song.Id
        };

        var room = await RoomService.ChangeCurrentSongAsync(changeCurrentSongModel);

        if (room is not null)
        {
            OnRoomUpdated(room);
        }
    }

    private async Task MoveToNextSong()
    {
        if (_roomModel?.Setlist?.Songs == null || _currentSong == null) return;

        var orderedSongs = _roomModel.Setlist.Songs.OrderBy(s => s.Order).ToList();
        var currentIndex = orderedSongs.FindIndex(s => s.Id == _currentSong.Id);

        if (currentIndex >= 0 && currentIndex < orderedSongs.Count - 1)
        {
            var nextSong = orderedSongs[currentIndex + 1];
            await SelectSong(nextSong);
        }
    }

    private async Task MoveToPrevSong()
    {
        if (_roomModel?.Setlist?.Songs == null || _currentSong == null) return;

        var orderedSongs = _roomModel.Setlist.Songs.OrderBy(s => s.Order).ToList();
        var currentIndex = orderedSongs.FindIndex(s => s.Id == _currentSong.Id);

        if (currentIndex > 0)
        {
            var prevSong = orderedSongs[currentIndex - 1];
            await SelectSong(prevSong);
        }
    }

    private async Task ScrollToCurrentSong()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync(_scrollToCurrentSongMethod);
        }
        catch
        {
            Snackbar.Add("Failed to scroll to current song.", Severity.Warning);
        }
    }

    private async Task OpenSetlistContentDialog()
    {
        if (_roomModel is null || _roomModel.Setlist is null || _currentSong is null) return;

        var parameters = new DialogParameters
        {
            { nameof(ShowSetlistContentDialog.Setlist), _roomModel.Setlist },
            { nameof(ShowSetlistContentDialog.CurrentSongId), _currentSong.Id }
        };

        var options = new DialogOptions { CloseButton = true };

        await DialogService.ShowAsync<ShowSetlistContentDialog>("Setlist Content", parameters, options);
    }

    private async Task OpenQrCodeDialog()
    {
        var roomUrl = NavigationManager.Uri;

        var parameters = new DialogParameters
        {
            { "RoomUrl", roomUrl }
        };

        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        await DialogService.ShowAsync<QrCodeDialog>("Room Access", parameters, options);
    }

    public async ValueTask DisposeAsync()
    {
        RoomService.RoomUpdated -= OnRoomUpdated;

        if (_jsModule != null)
        {
            await _jsModule.DisposeAsync();
        }
    }
}