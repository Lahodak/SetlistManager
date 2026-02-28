using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SetlistManager.App.Pages.Dialogs;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;
using SetlistManager.App.Models;

namespace SetlistManager.App.Pages;

public partial class Room : IDisposable
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

    private RoomModel? _roomModel;
    private SongModel? _currentSong;
    private UserModel? _user;
    private GeniusEmbedModel? _lyricsData;

    private bool _isFullscreen = false;
    private bool _drawerOpen = false;
    private bool _isLoadingLyrics = false;
    private bool _isScrolling = false;


    private List<PanelType> _activePanels = new() { PanelType.Song, PanelType.Setlist };
    private int _scrollSpeed = 5;
    private double _fontScale = 1.0;
    private string _currentTime = DateTime.Now.ToString("HH:mm:ss");
    private Timer? _clockTimer;

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

        await LoadPanelConfigFromStorage();

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

        if (_roomModel.Setlist is null)
            return;

        _currentSong = _roomModel.Setlist.Songs.FirstOrDefault(x => x.Id == _roomModel.CurrentSong);
        _roomModel.CurrentSong = _currentSong?.Id;

        _user = await UserService.GetUserAsync();

        await LoadLyricsIfNeeded();

        StateHasChanged();
        await ScrollToCurrentSong();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (_lyricsData is not null && (_previousSongId != _currentSong?.Id || _needsLyricsReload))
        {
            try
            {
                await JSRuntime.InvokeVoidAsync("window.geniusEmbed.loadEmbed", _lyricsData.SongId, _lyricsData.Title, _lyricsData.Artist, _lyricsData.Url);
                _previousSongId = _currentSong?.Id;
                _needsLyricsReload = false;
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error loading Genius embed: {ex.Message}");
            }
        }
    }

    private async Task LoadPanelConfigFromStorage()
    {
        var panels = await UserService.GetPanelConfigAsync();

        if(panels != null && panels.Count > 0)
        {
            _activePanels = panels;
        }        
    }

    private async Task SavePanelConfigToStorage() 
        => await UserService.SavePanelConfigAsync(_activePanels);

    private async Task TogglePanel(PanelType panel)
    {
        if (_activePanels.Contains(panel))
        {
            if (_activePanels.Count > 1)
            {
                _activePanels.Remove(panel);
            }
        }
        else
        {
            if (_activePanels.Count < 3)
            {
                _activePanels.Add(panel);
            }
        }

        if (_activePanels.Contains(PanelType.Lyrics))
        {
            if (_lyricsData != null)
            {
                _needsLyricsReload = true;
            }
            await LoadLyricsIfNeeded();
        }

        await SavePanelConfigToStorage();
        StateHasChanged();
    }

    private async Task MovePanel(PanelType panel, bool moveLeft)
    {
        var index = _activePanels.IndexOf(panel);
        if (index == -1)
            return;

        var newIndex = moveLeft
            ? index - 1
            : index + 1;

        if (newIndex < 0 || newIndex >= _activePanels.Count)
            return;

        _activePanels.RemoveAt(index);
        _activePanels.Insert(newIndex, panel);

        if (_activePanels.Contains(PanelType.Lyrics))
        {
            _needsLyricsReload = true;
        }

        await SavePanelConfigToStorage();
        StateHasChanged();
    }

    private void ToggleDrawer() 
        => _drawerOpen = !_drawerOpen;

    private async Task ToggleFullscreen()
    {
        try
        {
            _isFullscreen = await JSRuntime.InvokeAsync<bool>("toggleFullscreen");
            _drawerOpen = false;
            StateHasChanged();
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

        if (newCurrentSong?.Id != _currentSong?.Id)
        {
            _currentSong = newCurrentSong;

            await LoadLyricsIfNeeded();
            await ScrollToCurrentSong();
        }

        StateHasChanged();
    }

    private async Task LoadLyricsIfNeeded()
    {
        if (_currentSong != null && _activePanels.Contains(PanelType.Lyrics) && (_lyricsData == null || _previousSongId != _currentSong.Id))
        {
            _isLoadingLyrics = true;
            _lyricsData = null;
            StateHasChanged();
            
            _lyricsData = await GeniusService.FetchSongLyricsAsync(_currentSong);
            _needsLyricsReload = true;
            
            _isLoadingLyrics = false;
            StateHasChanged();            
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
        if (_roomModel?.Setlist?.Songs == null || _currentSong == null) 
            return;

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
        if (_roomModel?.Setlist?.Songs == null || _currentSong == null) 
            return;

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
        if (_roomModel?.Setlist?.Songs == null || _currentSong == null) 
            return false;

        var orderedSongs = _roomModel.Setlist.Songs
            .OrderBy(s => s.Order)
            .ToList();
        
        var currentIndex = orderedSongs.FindIndex(s => s.Id == _currentSong.Id);

        return currentIndex >= 0 && currentIndex < orderedSongs.Count - 1;
    }

    private bool CanMoveToPrevSong()
    {
        if (_roomModel?.Setlist?.Songs == null || _currentSong == null) 
            return false;

        var orderedSongs = _roomModel.Setlist.Songs
            .OrderBy(s => s.Order)
            .ToList();
        
        var currentIndex = orderedSongs.FindIndex(s => s.Id == _currentSong.Id);

        return currentIndex > 0;
    }

    private async Task ScrollToCurrentSong() 
        => await JSRuntime.InvokeVoidAsync("window.scrollToCurrentSong");

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

    private async Task CopyRoomCode()
    {
        await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", RoomCode);
        Snackbar.Add($"Room code \"{RoomCode}\" copied to clipboard!", Severity.Success);
    }

    protected override void OnInitialized()
    {
        _clockTimer = new Timer(_ =>
        {
            _currentTime = DateTime.Now.ToString("HH:mm:ss");
            InvokeAsync(StateHasChanged);
        }, null, 0, 1000);
    }

    public void Dispose() 
        => _clockTimer?.Dispose();

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
}