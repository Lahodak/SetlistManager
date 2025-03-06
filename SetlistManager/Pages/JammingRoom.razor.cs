namespace SetlistManager.Pages;
using Microsoft.AspNetCore.Components;
using SetlistManager.Services;
using SetlistManager.Common.Models;
using MudBlazor;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public partial class JammingRoom
{
    [Inject]
    public required SetlistService SetlistService { get; set; }
    [Inject]
    public required UserService UserService { get; set; }
    public RoomModel Room;

    protected async override Task OnInitializedAsync()
    {
        await FillRoomSampleData();
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
}