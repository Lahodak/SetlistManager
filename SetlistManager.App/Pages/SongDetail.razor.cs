using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class SongDetail
{
	[Parameter]
	public int SongId { get; set; }
    [Inject]
	public required ISongService SongService { get; set; }
	[Inject]
	public required IGeniusService GeniusService { get; set; }

	private string? _lyrics;
    private SongModel? _song;

	protected override async Task OnInitializedAsync()
	{	
		_song = await SongService.GetSongByIdAsync(SongId);

		if(_song is not null)
			_lyrics = await GeniusService.FetchSongLyricsAsync(_song);
		if (_lyrics is not null)
			StateHasChanged();
	}
}