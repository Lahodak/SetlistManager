using Microsoft.AspNetCore.Components;
using SetlistManager.Common.Models;
using SetlistManager.Models;
using SetlistManager.Services;

namespace SetlistManager.Pages;

public partial class SongDetail
{
	[Parameter]
	public int SongId { get; set; }
    [Inject]
	public required SongsDB SongsDatabase { get; set; }   	
    [Inject]
    public required LyricsService LyricsService { get; set; }

	private SongLyrics? songLyrics = new();
    SongModel song = new();

    protected override async Task OnInitializedAsync()
	{
        if(SongsDatabase.GetCount() == 0)
            await SongsDatabase.CheckForData();
		song = SongsDatabase.GetSong(SongId)!;
		if (song.Language.Code == "EN")
		{
			await SearchLyrics();
		}
	}    

	private async Task SearchLyrics()
	{
		songLyrics = await LyricsService.SearchLyricsAsync(song);
	}

	private MarkupString GetFormattedLyrics()
	{
        if (songLyrics?.Lyrics != null)
        {
            var formattedLyrics = songLyrics.Lyrics
                .Replace("\r\n", "<br/>")
                .Replace("\n", "<br/>")
                .Replace("\r", "<br/>")
                .Trim();

            formattedLyrics = System.Text.RegularExpressions.Regex.Replace(formattedLyrics, @"(<br/>)+", "<br/>");

            formattedLyrics = System.Text.RegularExpressions.Regex.Replace(formattedLyrics, @"\s+", " ");

            return (MarkupString)formattedLyrics;
        }

        return new MarkupString(string.Empty);
    }
}