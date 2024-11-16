using Microsoft.AspNetCore.Components;
using SetlistManager.Services;
using SetlistManager.Common.Models;

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
    
    [Inject]
    public required SongsDB SongsDatabase { get; set; }
    [Inject]
    public required SetlistService SetlistService { get; set; }

    private void ShowGenerateSetlistUI()
    {
        _showGenerateSetlistUI = true;
        _showLoadSetlistUI = false;
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
        => _showSetlistContentUI = false;
    private void GenerateSetlist()
	{
        ShowSetlistContentUI();
        _showSaveSetlistUI = true;
		_shuffeledSongCollection.AddRange(_songCollection);
        ShuffleService.ShuffleList(_shuffeledSongCollection);
		_shuffeledSongCollection = _shuffeledSongCollection.Take(_setlistLength).ToList();
	}

    private async Task GetSetlist()
    {
        if (_setlistToBeLoadedId <= 0)
            return;               
        _setlist = await SetlistService.GetSetlistById(_setlistToBeLoadedId)!;
        _shuffeledSongCollection.Clear();
        _shuffeledSongCollection.AddRange(_setlist.Songs);
        ShowSetlistContentUI();
    }

    private async Task SaveSetlist()
    {
        if (_toBeSavedSetlistName is null || _toBeSavedSetlistName.Length < 4)
            return;
        _setlist.Name = _toBeSavedSetlistName;
        _setlist.Songs.AddRange(_shuffeledSongCollection);
        await SetlistService.PushSetlist(_setlist);
    }

    protected override async Task OnInitializedAsync()
    {
        _songCollection.AddRange(await SongsDatabase.GetSongCollection());
        _maxNumber = SongsDatabase.GetCount();
    }

    private void RegenerateSong(int songId)
	{
        if (_shuffeledSongCollection.Count >= SongsDatabase.GetCount())
            return;
        int index = _shuffeledSongCollection.FindIndex(song => song.Id == songId);
        var availableSongs = _songCollection
                             .Where(song => !_shuffeledSongCollection.Contains(song) && song.Id != songId)
                             .ToList();
        SongModel newSong;
        if (availableSongs.Count <= 0)
            return;
        var random = new Random();
        newSong = availableSongs[random.Next(availableSongs.Count)];
        ReplaceSong(index, newSong);
        StateHasChanged();        
	}

	private void ReplaceSong(int index, SongModel newSong)
	{
		if (index >= 0 && index < _shuffeledSongCollection.Count)
		{
			_shuffeledSongCollection[index] = newSong;
		}
	}
}