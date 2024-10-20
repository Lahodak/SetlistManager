using Microsoft.AspNetCore.Components;
using SetlistManager.Models;
using SetlistManager.Services;

namespace SetlistManager.Pages;

public partial class SongDetail
{
	[Parameter]
	public int SongId { get; set; }
	Song song = new();
	[Inject]
	public SongsDB SongsDatabase { get; set; }

    protected override void OnInitialized()
	{
		song = SongsDatabase.GetSong(SongId);

		/*if (song != null)
		{
			await SearchLyrics();
		}*/
	}

    /*
	[Inject]
    public LyricsService LyricsService { get; set; }
	private SongLyrics songLyrics;

	private async Task SearchLyrics()
	{
		songLyrics = await LyricsService.SearchLyricsAsync(song);
	}
	private MarkupString GetFormattedLyrics()
	{
		if (songLyrics?.Lyrics != null)
		{
			var formattedLyrics = songLyrics.Lyrics.Replace("\n", "<br/>");
			return (MarkupString)formattedLyrics;
		}
		return new MarkupString(string.Empty);
	}
	*/
}