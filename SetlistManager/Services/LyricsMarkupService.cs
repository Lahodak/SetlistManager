using Microsoft.AspNetCore.Components;
using SetlistManager.Models;

namespace SetlistManager.Services;

public class LyricsMarkupService
{
    public MarkupString GetFormattedLyrics(SongLyrics songLyrics)
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