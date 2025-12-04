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

    private int _length = 1;
    private int _maxNumber = 0;
    private List<SongModel>? _allSongs = [];
    private List<SongModel> _shuffeledSongCollection = [];
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

    private void Generate()
    {
        if (_length <= 0 || _length > _maxNumber)
        {
            Snackbar.Add("Invalid setlist length", Severity.Error);
            return;
        }

        _shuffeledSongCollection.Clear();

        if (_allSongs is null)
        {
            Snackbar.Add("Couldn't find any available songs", Severity.Error);
            return;
        }

        _shuffeledSongCollection.AddRange(_allSongs);
        ShuffleService.ShuffleList(_shuffeledSongCollection);
        _shuffeledSongCollection = _shuffeledSongCollection.Take(_length).ToList();

        for (int i = 0; i < _shuffeledSongCollection.Count; i++)
        {
            _shuffeledSongCollection[i].Order = i + 1;
        }

        _setlist = new SetlistModel
        {
            Name = $"Generated {DateTime.Now:HH:mm}",
            Songs = _shuffeledSongCollection
        };

        _showSetlistContentUI = true;
        _showSaveSetlistUI = true;
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
            Snackbar.Add("Setlist name has to be 4 words or longer", Severity.Warning);
            return;
        }

        for (int i = 0; i < _shuffeledSongCollection.Count; i++)
        {
            _shuffeledSongCollection[i].Order = i + 1;
        }

        _setlist.Name = _toBeSavedSetlistName;

        _setlist.CreatorId = _user!.Id;

        if (_setlist is null)
            return;

        await SetlistService.PushSetlist(_setlist);
        MudDialog.Close(DialogResult.Ok(_setlist));
    }

    private void RegenerateSong(int songId)
    {
        if (_shuffeledSongCollection.Count >= _allSongs!.Count)
            return;

        int index = _shuffeledSongCollection.FindIndex(song => song.Id == songId);

        var availableSongs = new List<SongModel>();

        availableSongs.AddRange(GetAvailableSongs());

        SongModel newSong;

        if (availableSongs.Count <= 0)
            return;

        var random = new Random();

        newSong = availableSongs[random.Next(availableSongs.Count)];

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