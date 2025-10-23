using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class SongDetail
{
	[Parameter]
	public int SongId { get; set; }
    [Inject]
	public required SongsDB SongsDatabase { get; set; }
	[Inject]
	public required GeniusService GeniusService { get; set; }

	private string _lyrics;
    private SongModel? _song;

	protected override async Task OnInitializedAsync()
	{
		if (SongsDatabase.GetCount() == 0)
			await SongsDatabase.CheckForData();

		_song = SongsDatabase.GetSong(SongId)!;

		if(_song is not null)
			_lyrics = await GeniusService.FetchSongLyricsAsync(_song);
		if (_lyrics is not null)
			StateHasChanged();
	}
}