namespace SetlistManager.Pages;
using Microsoft.AspNetCore.Components;
using SetlistManager.Services;
using SetlistManager.Common.Models;
using System.Threading.Tasks;
using SetlistManager.Models;

public partial class JammingRoom
{
    [Inject]
    public required SetlistService SetlistService { get; set; }
    [Inject]
    public required UserService UserService { get; set; }
    [Inject]
    public required LyricsService LyricsService { get; set; }
    [Inject]
    public required LyricsMarkupService LyricsMarkupService { get; set; }

    public RoomModel Room;
    private SongLyrics SongLyrics = new();

    //protected async override Task OnInitializedAsync()
    //{
        
    //}

    private async Task NextSong()
    {
        if (Room.Setlist is null)
            return;
        if (Room.CurrentSong == Room.Setlist.Songs.Count - 1)
            return;
        Room.CurrentSong++;
        await GetLyrics();
    }

    private async Task PreviousSong()
    {
        if (Room.CurrentSong == 0)
            return;
        Room.CurrentSong--;
        await GetLyrics();
    }
    private async Task GetLyrics()
    {
        SongLyrics = await LyricsService.SearchLyricsAsync(Room.Setlist.Songs[Room.CurrentSong]);
    }
}