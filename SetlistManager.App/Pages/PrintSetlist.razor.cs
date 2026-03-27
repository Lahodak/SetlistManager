using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class PrintSetlist
{
    [Parameter]
    public int SetlistId { get; set; }
    [Inject]
    public required ISetlistService SetlistService { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }
    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    private SetlistModel? _setlist;
    private bool _includeSongOrder = true;
    private bool _includeSongName = true;
    private bool _includeArtist = true;
    private bool _includeTuning = false;
    private bool _includeBPM = false;
    private bool _includeKey = false;

    protected override async Task OnInitializedAsync()
    {
        _setlist = await SetlistService.GetSetlistById(SetlistId);
    }

    private string GenerateSongLine(SongModel song)
    {
        var parts = new List<string>();

        if (_includeSongOrder)
        {
            parts.Add($"{song.Order}.");
        }

        if (_includeSongName)
        {
            parts.Add(song.Name);
        }

        if (_includeArtist && !string.IsNullOrEmpty(song.Artist?.Nick))
        {
            parts.Add($"({song.Artist.Nick})");
        }

        if (_includeTuning && !string.IsNullOrEmpty(song.Tuning))
        {
            parts.Add($"[{song.Tuning}]");
        }

        if (_includeKey && !string.IsNullOrEmpty(song.Key))
        {
            parts.Add($"[{song.Key}]");
        }

        if (_includeBPM && song.BPM > 0)
        {
            parts.Add($"[{song.BPM}]");
        }

        return string.Join(" ", parts);
    }

    private async Task HandlePrint()
    {
        await JSRuntime.InvokeVoidAsync("window.print");
    }

    private void GoBack()
    {
        NavigationManager.NavigateTo($"/setlists/{SetlistId}");
    }
}