using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SetlistManager.App.Pages.Dialogs;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;
using SetlistManager.App.Models;

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

    [Inject]
    public required IGeniusService GeniusService { get; set; }

    private const string _roomsPortalUri = "/RoomsPortal";
    private const string _viewModeStorageKey = "roomViewMode";

    private RoomModel? _roomModel;
    private SongModel? _currentSong;
    private UserModel? _user;
    private GeniusEmbedModel? _lyricsData;

    private bool _isFullscreen = false;
    private bool _drawerOpen = false;
    private bool _isLoadingLyrics = false;
    private bool _isScrolling = false;

    private ViewMode _currentViewMode = ViewMode.SongAndSetlist;
    private int _scrollSpeed = 5;
    private double _fontScale = 1.0;

    private IJSObjectReference? _jsModule;
    private int? _previousSongId;
    private bool _needsLyricsReload = false;

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrWhiteSpace(RoomCode))
        {
            Snackbar.Add("RoomCode cannot be blank!", Severity.Error);
            NavigationManager.NavigateTo(_roomsPortalUri);
            return;
        }

        // Load saved view mode from localStorage
        await LoadViewModeFromStorage();

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

        // Load lyrics if needed
        await LoadLyricsIfNeeded();

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

        // Render lyrics if available and needs reload
        if (_lyricsData is not null && (_previousSongId != _currentSong?.Id || _needsLyricsReload))
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("window.geniusEmbed.loadEmbed",
                    _lyricsData.SongId, _lyricsData.Title, _lyricsData.Artist, _lyricsData.Url);
                _previousSongId = _currentSong?.Id;
                _needsLyricsReload = false;
            }
            catch
            {
                // Lyrics embed failed, silently continue
            }
        }
    }

    private async Task LoadViewModeFromStorage()
    {
        try
        {
            var savedMode = await JSRuntime.InvokeAsync<string>("localStorage.getItem", _viewModeStorageKey);
            if (!string.IsNullOrEmpty(savedMode) && Enum.TryParse<ViewMode>(savedMode, out var viewMode))
            {
                _currentViewMode = viewMode;
            }
        }
        catch
        {
            // If localStorage fails, use default
        }
    }

    private async Task SaveViewModeToStorage()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("localStorage.setItem", _viewModeStorageKey, _currentViewMode.ToString());
        }
        catch
        {
            // If localStorage fails, continue without saving
        }
    }

    private async Task OnViewModeChanged(ViewMode newMode)
    {
        _currentViewMode = newMode;
        await SaveViewModeToStorage();

        // Check if new view mode has lyrics
        var currentHasLyrics = newMode == ViewMode.SongAndLyrics ||
                               newMode == ViewMode.LyricsAndSetlist ||
                               newMode == ViewMode.LyricsOnly;

        // If switching to a lyrics view and we already have lyrics data, mark for reload
        if (currentHasLyrics && _lyricsData != null)
        {
            _needsLyricsReload = true;
        }

        // Load lyrics if switching to a lyrics view
        await LoadLyricsIfNeeded();

        _drawerOpen = false;
        StateHasChanged();
    }

    private void ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;
    }

    private async Task ToggleFullscreen()
    {
        try
        {
            if (_jsModule != null)
            {
                _isFullscreen = await _jsModule.InvokeAsync<bool>("toggleFullscreen");
                _drawerOpen = false;
                StateHasChanged();
            }
        }
        catch
        {
            Snackbar.Add("Fullscreen mode is not supported by your browser.", Severity.Warning);
        }
    }

    private async void OnRoomUpdated(RoomModel room)
    {
        _roomModel = room;

        if (_roomModel?.Setlist is null || _roomModel.CurrentSong is null)
            return;

        var newCurrentSong = _roomModel.Setlist.Songs.FirstOrDefault(x => x.Id == _roomModel.CurrentSong);

        // Check if song changed
        if (newCurrentSong?.Id != _currentSong?.Id)
        {
            _currentSong = newCurrentSong;

            // Load lyrics for new song if in lyrics view
            await LoadLyricsIfNeeded();
            await ScrollToCurrentSong();
        }

        StateHasChanged();
    }

    private async Task LoadLyricsIfNeeded()
    {
        // Only load lyrics if current view mode includes lyrics
        if (_currentSong != null &&
            (_currentViewMode == ViewMode.SongAndLyrics ||
             _currentViewMode == ViewMode.LyricsAndSetlist ||
             _currentViewMode == ViewMode.LyricsOnly))
        {
            // Only reload if song changed
            if (_lyricsData == null || _previousSongId != _currentSong.Id)
            {
                _isLoadingLyrics = true;
                _lyricsData = null;
                StateHasChanged();

                try
                {
                    _lyricsData = await GeniusService.FetchSongLyricsAsync(_currentSong);
                    _needsLyricsReload = true; // Mark for reload in AfterRender
                }
                catch
                {
                    // Lyrics fetch failed
                }
                finally
                {
                    _isLoadingLyrics = false;
                    StateHasChanged();
                }
            }
        }
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

    private bool CanMoveToNextSong()
    {
        if (_roomModel?.Setlist?.Songs == null || _currentSong == null) return false;

        var orderedSongs = _roomModel.Setlist.Songs.OrderBy(s => s.Order).ToList();
        var currentIndex = orderedSongs.FindIndex(s => s.Id == _currentSong.Id);

        return currentIndex >= 0 && currentIndex < orderedSongs.Count - 1;
    }

    private bool CanMoveToPrevSong()
    {
        if (_roomModel?.Setlist?.Songs == null || _currentSong == null) return false;

        var orderedSongs = _roomModel.Setlist.Songs.OrderBy(s => s.Order).ToList();
        var currentIndex = orderedSongs.FindIndex(s => s.Id == _currentSong.Id);

        return currentIndex > 0;
    }

    private async Task ScrollToCurrentSong()
    {
        try
        {
            await JSRuntime.InvokeVoidAsync("window.scrollToCurrentSong");
        }
        catch
        {
            // Scroll failed, silently continue
        }
    }

    // Lyrics autoscroll methods
    private async Task ToggleScroll(bool scroll)
    {
        _isScrolling = scroll;
        if (_isScrolling)
        {
            await JSRuntime.InvokeVoidAsync("window.scrollingFunctions.startAutoScroll", "genius-lyrics-container", _scrollSpeed);
        }
        else
        {
            await JSRuntime.InvokeVoidAsync("window.scrollingFunctions.stopAutoScroll");
        }
        StateHasChanged();
    }

    private async Task ResetScroll()
    {
        await JSRuntime.InvokeVoidAsync("eval", "document.getElementById('genius-lyrics-container').scrollTop = 0");
        if (_isScrolling)
        {
            await ToggleScroll(true);
        }
    }

    private async Task OnSpeedChanged(int newSpeed)
    {
        _scrollSpeed = newSpeed;
        if (_isScrolling)
        {
            await JSRuntime.InvokeVoidAsync("window.scrollingFunctions.startAutoScroll", "genius-lyrics-container", _scrollSpeed);
        }
    }

    private void OnFontScaleChanged(double newScale)
    {
        _fontScale = newScale;
        StateHasChanged();
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

        _drawerOpen = false;
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

public enum ViewMode
{
    SongAndSetlist,
    SongAndLyrics,
    LyricsAndSetlist,
    SongOnly,
    LyricsOnly,
    SetlistOnly
}