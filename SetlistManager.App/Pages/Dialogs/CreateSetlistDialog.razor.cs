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
    private string? _toBeSavedSetlistName;

    protected override async Task OnInitializedAsync()
    {
        _allSongs = (await SongService.GetSongsAsync(new()
        {
            PageSize = int.MaxValue
        }))?.Items;

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
        ClearSetlist();
        StateHasChanged();
    }

    private string GetGenerationHelperText()
    {
        if (_manuallyAddedSongs.Count == 0)
            return "All songs will be randomly selected from your Library";

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

        ReorderSongs();

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
            _toBeSavedSetlistName = $"Manual {DateTime.Now:HH:mm:ss}";
        }

        _showSaveSetlistUI = true;
    }

    private void Generate()
    {
        if (!ValidateGenerationPreconditions())
            return;

        var generatedSongs = GenerateSongList();

        UpdateShuffledCollection(generatedSongs);
        SetDefaultNameIfEmpty();
        ShowUI();
        ShowWarningIfIncomplete(generatedSongs.Count);

        StateHasChanged();
    }

    private bool ValidateGenerationPreconditions()
    {
        if (_allSongs is null)
        {
            Snackbar.Add("Couldn't find any available songs", Severity.Error);
            return false;
        }

        int finalLength = Math.Min(_length, _maxNumber);
        if (finalLength <= 0)
        {
            Snackbar.Add("Invalid setlist length", Severity.Error);
            return false;
        }

        return true;
    }

    private List<SongModel> GenerateSongList()
    {
        int finalLength = Math.Min(_length, _maxNumber);
        var manualSongs = new List<SongModel>(_manuallyAddedSongs);
        int remainingSlots = Math.Max(0, finalLength - manualSongs.Count);

        var randomSongs = GetRandomSongs(manualSongs, remainingSlots);

        var finalList = new List<SongModel>(manualSongs);
        finalList.AddRange(randomSongs);

        return finalList;
    }

    private List<SongModel> GetRandomSongs(List<SongModel> manualSongs, int count)
    {
        if (count <= 0 || _allSongs is null)
            return [];

        var availablePool = _allSongs
            .Where(s => !manualSongs.Any(ms => ms.Id == s.Id))
            .ToList();

        if (availablePool.Count == 0)
            return [];

        ShuffleService.ShuffleList(availablePool);
        return availablePool.Take(count).ToList();
    }

    private void UpdateShuffledCollection(List<SongModel> songs)
    {
        _shuffeledSongCollection.Clear();
        _shuffeledSongCollection.AddRange(songs);
        ReorderSongs();
    }

    private void SetDefaultNameIfEmpty()
    {
        if (string.IsNullOrWhiteSpace(_toBeSavedSetlistName))
        {
            _toBeSavedSetlistName = $"Generated {DateTime.Now:HH:mm:ss}";
        }
    }

    private void ShowUI()
    {
        _showSetlistContentUI = true;
        _showSaveSetlistUI = true;
    }

    private void ShowWarningIfIncomplete(int actualCount)
    {
        int requestedLength = Math.Min(_length, _maxNumber);
        if (actualCount != requestedLength)
        {
            Snackbar.Add(
                $"Could only generate {actualCount} songs (Max available: {_maxNumber})",
                Severity.Warning
            );
        }
    }

    private async Task Save()
    {
        if (_toBeSavedSetlistName is null)
        {
            Snackbar.Add("Fill out the name first", Severity.Warning);
            return;
        }

        if (_toBeSavedSetlistName.Length < 4)
        {
            Snackbar.Add("Setlist name has to be 4 characters or longer", Severity.Warning);
            return;
        }

        ReorderSongs();

        var createModel = new SetlistCreateModel
        {
            Name = _toBeSavedSetlistName,
            Songs = _shuffeledSongCollection
                .Select(s => new SetlistSongOrderItem { SongId = s.Id, Order = s.Order })
                .ToList()
        };

        var result = await SetlistService.TryCreateSetlistAsync(createModel);
        
        if (!result)
        {
            Snackbar.Add("Failed to save setlist", Severity.Error);
            return;
        }

        Snackbar.Add("Setlist saved successfully", Severity.Success);
        MudDialog.Close();
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

        var availableSongs = GetAvailableSongsExcludingManual();

        if (availableSongs.Count <= 0)
        {
            Snackbar.Add("No other songs available to regenerate with", Severity.Warning);
            return;
        }

        SongModel newSong = availableSongs[Random.Shared.Next(availableSongs.Count)];
        ReplaceSong(index, newSong);
        StateHasChanged();
    }

    private void ClearSetlist()
    {
        _shuffeledSongCollection.Clear();
        _manuallyAddedSongs.Clear();
        _showSetlistContentUI = false;
        _showSaveSetlistUI = false;
        _toBeSavedSetlistName = null;
    }

    private void ReorderSongs()
    {
        for (int i = 0; i < _shuffeledSongCollection.Count; i++)
        {
            _shuffeledSongCollection[i].Order = i + 1;
        }
    }

    private List<SongModel> GetAvailableSongs()
    {
        return (_allSongs ?? [])
            .Where(s => !_shuffeledSongCollection.Any(ss => ss.Id == s.Id))
            .ToList();
    }

    private List<SongModel> GetAvailableSongsExcludingManual()
    {
        return GetAvailableSongs()
            .Where(s => !_manuallyAddedSongs.Any(ms => ms.Id == s.Id))
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