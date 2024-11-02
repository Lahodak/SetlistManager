using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using SetlistManager.Services;
using System;
using SetlistManager.Common.Models;
using System.Net.Http.Json;

namespace SetlistManager.Pages;

public partial class Home
{	
	private int _maxNumber = 0;
	private int _setlistLength = 1;
	private List<SongModel> _shuffeledSongCollection = [];
	private List<SongModel> _uploadedSongs = [];
	private List<SongModel> _songCollection = [];

    [Inject]
    public required SongsDB SongsDatabase { get; set; }
    private async Task GenerateSetlist()
	{		
		_shuffeledSongCollection.AddRange(_songCollection);
        ShuffleService.ShuffleList(_shuffeledSongCollection);
		_shuffeledSongCollection = _shuffeledSongCollection.Take(_setlistLength).ToList();
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