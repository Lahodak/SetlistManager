using Microsoft.AspNetCore.Components;
using SetlistManager.Services;
using SetlistManager.Common.Models;
using MudBlazor;

namespace SetlistManager.Pages;

public partial class Home
{	
	private int _maxNumber = 0;
	private int _setlistLength = 1;
	private List<SongModel> _shuffeledSongCollection = [];
	private readonly List<SongModel> _songCollection = [];
    private SetlistModel _setlist = new();
    private bool _showGenerateSetlistUI;
    private bool _showSaveSetlistUI;
    private string? _toBeSavedSetlistName;
    private bool _showLoadSetlistUI;
    private int _setlistToBeLoadedId;
    private bool _showSetlistContentUI = true;
    private bool _showSetlistId = false;
    private bool _showSearchAndReplaceUI = false;
    private bool _setlistExists = true;
    private const string _localStorageKey = "LastLoadedSetlistId";
    
    [Inject]
    public required SongsDB SongsDatabase { get; set; }
    [Inject]
    public required SetlistService SetlistService { get; set; }
    [Inject]
    public required Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private void ShowGenerateSetlistUI()
    {
        _showGenerateSetlistUI = true;
        _showLoadSetlistUI = false;
        _shuffeledSongCollection.Clear();
    }        

    private void HideGenerateSetlistUI()
        => _showGenerateSetlistUI = false;

    private void ShowLoadSetlistUI()
    {
        _showLoadSetlistUI = true;
        _showGenerateSetlistUI = false;
        _showSaveSetlistUI = false;
        HideSetlistContentUI();
    }

    private void HideLoadSetlistUI()
        => _showLoadSetlistUI = false;
    private void ShowSetlistContentUI()
        => _showSetlistContentUI = true;
    private void HideSetlistContentUI()
    {
        _showSetlistContentUI = false;
        _showSetlistId = false;
    }

    private void GenerateSetlist()
	{
        ShowSetlistContentUI();
        _showSaveSetlistUI = true;
        _shuffeledSongCollection.Clear();
		_shuffeledSongCollection.AddRange(_songCollection);
        ShuffleService.ShuffleList(_shuffeledSongCollection);
		_shuffeledSongCollection = _shuffeledSongCollection.Take(_setlistLength).ToList();
    }

    private async Task GetSetlist()
    {
        _shuffeledSongCollection.Clear();
        if (_setlistToBeLoadedId <= 0)
            return;           
        
        _setlist = await SetlistService.GetSetlistById(_setlistToBeLoadedId)!;

        if (_setlist.Songs.Count == 0)
        {
            _setlistExists = false;
            Snackbar.Add($"Setlist with ID: {_setlistToBeLoadedId} doesn't exist!");
            return;
        }           
        else
        {
            _setlistExists = true;
        }

        _shuffeledSongCollection.Clear();
        _shuffeledSongCollection.AddRange(_setlist.Songs);
        ShowSetlistContentUI();
        await LocalStorage.SetItemAsync(_localStorageKey, _setlistToBeLoadedId);
    }

    protected override async Task OnInitializedAsync()
    {
        _songCollection.AddRange(await SongsDatabase.GetSongCollection());
        _maxNumber = SongsDatabase.GetCount();
        var localData = await LocalStorage.GetItemAsync<string>(_localStorageKey);

        if(!int.TryParse(localData, out int lastLoadedSetlistId))        
            return;    
        
        if(lastLoadedSetlistId <= 0)
        {
            return;
        }
        else
        {
            _setlistToBeLoadedId = lastLoadedSetlistId;
            await GetSetlist();
            ShowLoadSetlistUI();
        }
    }

    private async Task SaveSetlist()
    {
        if (_toBeSavedSetlistName is null || _toBeSavedSetlistName.Length < 4)
            return;

        _setlist.Songs.Clear();
        _setlist.Name = _toBeSavedSetlistName;
        _setlist.Songs.AddRange(_shuffeledSongCollection);
        _setlist.Id = await SetlistService.PushSetlist(_setlist);
        _showSetlistId = true;
        await LocalStorage.SetItemAsync(_localStorageKey, _setlist.Id);
    }

    private void RegenerateSong(int songId)
	{
        if (_shuffeledSongCollection.Count >= SongsDatabase.GetCount())
            return;

        int index = _shuffeledSongCollection.FindIndex(song => song.Id == songId);

        var availableSongs = new List<SongModel>();
        
        availableSongs.AddRange(GetAvailableSongs());

        SongModel newSong = new();

        if (availableSongs.Count <= 0)
            return;

        var random = new Random();
        newSong = availableSongs[random.Next(availableSongs.Count)];
        ReplaceSong(index, newSong);
        StateHasChanged();        
	}

    private List<SongModel> GetAvailableSongs()
    {
        List<SongModel> availableSongs = [];

        foreach (var song in _songCollection)
        {
            if (!_shuffeledSongCollection.Any(shuffledSong => shuffledSong.Id == song.Id))  
            {
                availableSongs.Add(song);
            }
        }

        return availableSongs;
    }

    private void SearchAndReplaceSong(int id)
    {
        _showSearchAndReplaceUI = true;
        var availableSongs = GetAvailableSongs();
    }


	private void ReplaceSong(int index, SongModel newSong)
	{
		if (index >= 0 && index < _shuffeledSongCollection.Count)
		{
			_shuffeledSongCollection[index] = newSong;
		}
	}
}