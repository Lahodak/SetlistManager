using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using SetlistManager.Models;
using SetlistManager.Services;

namespace SetlistManager.Pages;

public partial class Home
{
	private int _maxNumber = 0;
	private int _setlistLength = 0;
	private List<Song>? _shuffeledSongCollection = [];
	private List<Song> _uploadedSongs = [];
	private List<Song> _songCollection = [];
    [Inject]
    public SongsDB SongsDatabase { get; set; }
    private void GenerateSetlist()
	{
		_shuffeledSongCollection = SongsDatabase.GetSongCollection();
		_songCollection = SongsDatabase.GetSongCollection();
        ShuffleService.ShuffleList(_shuffeledSongCollection);
		_shuffeledSongCollection = _shuffeledSongCollection.Take(_setlistLength).ToList();
	}
		
	private async Task HandleFileUpload(InputFileChangeEventArgs e)
	{
		var file = e.File;

		if (file != null)
		{
			var fileStream = file.OpenReadStream();
			try
			{
				_uploadedSongs = await CsvService.ReadCsvFile(fileStream);
                SongsDatabase.AddSongsToSongsDB(_uploadedSongs);
                _maxNumber = SongsDatabase.GetCount();
			}
			catch (ApplicationException ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}

	private void RegenerateSong(int songId)
	{
		if (_shuffeledSongCollection.Count == SongsDatabase.GetCount())
			return;
		int index = _shuffeledSongCollection.FindIndex(song => song.SongID == songId);
		Song newSong;
		do
		{
			ShuffleService.ShuffleList(_songCollection);
			newSong = _songCollection[0];
		} while (songId == newSong.SongID || _shuffeledSongCollection.Contains(newSong));
		ReplaceSong(index, newSong);
		StateHasChanged();
	}
	private void ReplaceSong(int index, Song newSong)
	{
		if (index >= 0 && index < _shuffeledSongCollection.Count)
		{
			_shuffeledSongCollection[index] = newSong;
		}
	}
}