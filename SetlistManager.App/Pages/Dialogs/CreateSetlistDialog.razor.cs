using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages.Dialogs;

public partial class CreateSetlistDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }
    [Inject]
    public required ISetlistService SetlistService { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }
    [Inject]
    public required ISongService SongService { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private enum CreationMode
    {
        Generate,
        Manual
    }

    private CreationMode _creationMode = CreationMode.Generate;
    private int _length = 1;
    private int _maxNumber = 0;
    private List<SongModel>? _allSongs = [];
    private readonly List<SongModel> _shuffeledSongCollection = [];
    private readonly List<SongModel> _manuallyAddedSongs = [];
    private bool _showSetlistContentUI;
    private bool _showSaveSetlistUI;
    private SetlistModel _setlist = new();
    private string? _toBeSavedSetlistName;
    private UserModel? _user = new();

    protected override async Task OnInitializedAsync()
    {
        _allSongs = (await SongService.GetAllSongsAsync(new() { PageSize = int.MaxValue}))?.Items;
        _user = await UserService.GetUserAsync();

        if (_user is null)
        {
            Snackbar.Add("Failed to verify user", Severity.Error);
            return;
        }

        if (_allSongs is null)
        {
            Snackbar.Add("Couldn't retrieve any songs", Severity.Error);
            _maxNumber = 0;
            return;
        }

        _maxNumber = _allSongs.Count;
    }

    private void OnCreationModeChanged(CreationMode newMode)
    {
        _creationMode = newMode;
        _shuffeledSongCollection.Clear();
        _manuallyAddedSongs.Clear();
        _showSetlistContentUI = false;
        _showSaveSetlistUI = false;
        _toBeSavedSetlistName = null;
        StateHasChanged();
    }

    private string GetGenerationHelperText()
    {
        if (_manuallyAddedSongs.Count == 0)
            return "All songs will be randomly selected";

        int randomCount = Math.Max(0, _length - _manuallyAddedSongs.Count);
        return $"{_manuallyAddedSongs.Count} specific + {randomCount} random songs";
    }

    private Task<IEnumerable<SongModel>> Search(string value, CancellationToken token)
    {
        if (_allSongs is null)
            return Task.FromResult<IEnumerable<SongModel>>([]);

        if (string.IsNullOrWhiteSpace(value))
        {
            var availableSongs = _allSongs
                .Where(s => !_shuffeledSongCollection.Any(ss => ss.Id == s.Id))
                .ToList();
            return Task.FromResult<IEnumerable<SongModel>>(availableSongs);
        }

        var searchResults = _allSongs
            .Where(s => (s.Name?.Contains(value, StringComparison.OrdinalIgnoreCase) ?? false) ||
                       (s.Artist?.Nick.Contains(value, StringComparison.OrdinalIgnoreCase) ?? false))
            .Where(s => !_shuffeledSongCollection.Any(ss => ss.Id == s.Id))
            .ToList();

        return Task.FromResult<IEnumerable<SongModel>>(searchResults);
    }

    private void OnSongSelected(SongModel selectedSong)
    {
        if (selectedSong == null) return;

        if (_shuffeledSongCollection.Any(s => s.Id == selectedSong.Id))
        {
            Snackbar.Add("Song already in setlist", Severity.Info);
            return;
        }

        if (!_manuallyAddedSongs.Any(s => s.Id == selectedSong.Id))
        {
            _manuallyAddedSongs.Add(selectedSong);
        }

        selectedSong.Order = _shuffeledSongCollection.Count + 1;
        _shuffeledSongCollection.Add(selectedSong);
        _showSetlistContentUI = true;
        StateHasChanged();
    }

    private void RemoveSongFromSetlist(SongModel songToRemove)
    {
        _shuffeledSongCollection.Remove(songToRemove);
        _manuallyAddedSongs.Remove(songToRemove);

        for (int i = 0; i < _shuffeledSongCollection.Count; i++)
        {
            _shuffeledSongCollection[i].Order = i + 1;
        }

        if (_shuffeledSongCollection.Count == 0)
        {
            _showSetlistContentUI = false;
            _showSaveSetlistUI = false;
        }

        StateHasChanged();
    }

    private void FinalizeManualSetlist()
    {
        if (_shuffeledSongCollection.Count == 0)
        {
            Snackbar.Add("Please add at least one song", Severity.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(_toBeSavedSetlistName))
        {
            _toBeSavedSetlistName = $"Manual {DateTime.Now:HH:mm}";
        }

        _setlist = new SetlistModel
        {
            Name = _toBeSavedSetlistName,
            Songs = _shuffeledSongCollection
        };

        _showSaveSetlistUI = true;
    }

    private void Generate()
    {
        if (_allSongs is null)
        {
            Snackbar.Add("Couldn't find any available songs", Severity.Error);
            return;
        }

        int finalLength = Math.Min(_length, _maxNumber);

        if (finalLength <= 0)
        {
            Snackbar.Add("Invalid setlist length", Severity.Error);
            return;
        }

        var currentManualSongs = new List<SongModel>(_manuallyAddedSongs);
        int remainingSlots = finalLength - currentManualSongs.Count;

        if (remainingSlots < 0)
        {
            remainingSlots = 0;
        }

        var shufflePool = (_allSongs ?? [])
            .Where(s => !currentManualSongs.Any(ms => ms.Id == s.Id))
            .ToList();

        var finalSetlist = new List<SongModel>(currentManualSongs);

        if (remainingSlots > 0 && shufflePool.Count > 0)
        {
            ShuffleService.ShuffleList(shufflePool);

            var randomSongs = shufflePool
                .Take(remainingSlots)
                .ToList();

            finalSetlist.AddRange(randomSongs);
        }

        _shuffeledSongCollection.Clear();
        _shuffeledSongCollection.AddRange(finalSetlist);

        for (int i = 0; i < _shuffeledSongCollection.Count; i++)
        {
            _shuffeledSongCollection[i].Order = i + 1;
        }

        if (string.IsNullOrWhiteSpace(_toBeSavedSetlistName))
        {
            _toBeSavedSetlistName = $"Generated {DateTime.Now:HH:mm}";
        }

        _setlist = new SetlistModel
        {
            Name = _toBeSavedSetlistName,
            Songs = _shuffeledSongCollection
        };

        _showSetlistContentUI = true;
        _showSaveSetlistUI = true;

        if (_shuffeledSongCollection.Count != finalLength)
        {
            Snackbar.Add($"Could only generate {_shuffeledSongCollection.Count} songs (Max available: {_maxNumber})", Severity.Warning);
        }

        StateHasChanged();
    }

    private async Task Save()
    {
        if (_toBeSavedSetlistName is null)
        {
            Snackbar.Add("Fill out the name first", Severity.Warning);
            return;
        }

        if(_user is null)
            return;
        
        if (_toBeSavedSetlistName.Length < 4)
        {
            Snackbar.Add("Setlist name has to be 4 characters or longer", Severity.Warning);
            return;
        }

        for (int i = 0; i < _shuffeledSongCollection.Count; i++)
        {
            _shuffeledSongCollection[i].Order = i + 1;
        }

        _setlist.Name = _toBeSavedSetlistName;            
        _setlist.OwnerId = _user.Id;
        _setlist.Songs = _shuffeledSongCollection;

        if (_setlist is null) return;

        await SetlistService.SaveSetlistAsync(_setlist);
        MudDialog.Close(DialogResult.Ok(_setlist));
    }

    private void RegenerateSong(int songId)
    {
        if (_shuffeledSongCollection.Count >= _allSongs!.Count) return;

        int index = _shuffeledSongCollection.FindIndex(song => song.Id == songId);

        if (_manuallyAddedSongs.Any(s => s.Id == songId))
        {
            Snackbar.Add("Cannot regenerate a manually added song", Severity.Warning);
            return;
        }

        var availableSongs = GetAvailableSongs()
            .Where(s => !_manuallyAddedSongs.Any(ms => ms.Id == s.Id))
            .ToList();

        if (availableSongs.Count <= 0)
        {
            Snackbar.Add("No other songs available to regenerate with", Severity.Warning);
            return;
        }

        SongModel newSong = availableSongs[Random.Shared.Next(availableSongs.Count)];
        ReplaceSong(index, newSong);
        StateHasChanged();
    }

    private List<SongModel> GetAvailableSongs()
    {
        return (_allSongs ?? [])
            .Where(s => !_shuffeledSongCollection.Any(ss => ss.Id == s.Id))
            .ToList();
    }

    private void ReplaceSong(int index, SongModel newSong)
    {
        if (index >= 0 && index < _shuffeledSongCollection.Count)
        {
            newSong.Order = index + 1;
            _shuffeledSongCollection[index] = newSong;
        }
    }

    private void Cancel() => MudDialog.Cancel();
}