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
    protected override async Task OnInitializedAsync()
	{
		song = SongsDatabase.GetSong(SongId);

		if (song.Language == Language.EN)
		{
			await SearchLyrics();
		}
	}
	private string GetEmbedUrl(string youtubeUrl)
	{
        var uri = new Uri(youtubeUrl);
        var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
        
		if (query.TryGetValue("v", out var videoId))
        {
            return $"https://www.youtube.com/embed/{videoId}";
        
		}
        if (uri.Host.Contains("youtu.be"))
        {
            return $"https://www.youtube.com/embed{uri.AbsolutePath}";
        }
        return youtubeUrl;
	}
    
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
            // Normalize newlines to <br/> and trim whitespace
            var formattedLyrics = songLyrics.Lyrics
                .Replace("\r\n", "<br/>")  // Replace Windows newlines
                .Replace("\n", "<br/>")     // Replace Unix newlines
                .Replace("\r", "<br/>")     // Replace old Mac newlines
                .Trim();

            // Replace multiple consecutive <br/> with a single <br/>
            formattedLyrics = System.Text.RegularExpressions.Regex.Replace(formattedLyrics, @"(<br/>)+", "<br/>");

            // Optionally, replace multiple spaces with a single space
            formattedLyrics = System.Text.RegularExpressions.Regex.Replace(formattedLyrics, @"\s+", " ");

            return (MarkupString)formattedLyrics;
        }
        return new MarkupString(string.Empty);
    }
}