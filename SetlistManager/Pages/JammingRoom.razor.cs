namespace SetlistManager.Pages;
using Microsoft.AspNetCore.Components;
using SetlistManager.Services;
using SetlistManager.Common.Models;
using MudBlazor;
using System.Runtime.CompilerServices;
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

    public RoomModel Room = new();
    private SongLyrics SongLyrics = new();

    protected async override Task OnInitializedAsync()
    {
        await FillRoomSampleData();
        await GetLyrics();
    }

    public async Task FillRoomSampleData()
    {
        Room = new()
        {
            Setlist = await SetlistService.GetSetlistById(5),
            Users = UserService.GetUsers(),
            Id = 1,
            Name = "JammingrRoom",
            CurrentSong = 0
        };
    }
    private async Task NextSong()
    {
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