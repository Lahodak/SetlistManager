using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;
using SetlistManager.App.Models;

namespace SetlistManager.App.Pages;

public partial class SongDetail
{
    [Parameter] 
    public int SongId { get; set; }
    [Inject] 
    public required ISongService SongService { get; set; }
    [Inject] 
    public required IGeniusService GeniusService { get; set; }
    [Inject] 
    public required IJSRuntime JSRuntime { get; set; }

    private SongModel? _song;
    private GeniusEmbedModel? _lyricsData;
    private int _scrollSpeed = 5;
    private bool _isScrolling = false;
    private double _fontScale = 1.0;

    protected override async Task OnInitializedAsync()
    {
        _song = await SongService.GetSongByIdAsync(SongId);
        if (_song is not null)
            _lyricsData = await GeniusService.FetchSongLyricsAsync(_song);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_lyricsData is not null)
        {
            await JSRuntime.InvokeVoidAsync("window.geniusEmbed.loadEmbed",
                _lyricsData.SongId, _lyricsData.Title, _lyricsData.Artist, _lyricsData.Url);
        }
    }

    private async Task ToggleScroll(bool scroll)
    {
        _isScrolling = scroll;
        if (_isScrolling)
            await JSRuntime.InvokeVoidAsync("window.scrollingFunctions.startAutoScroll", "genius-lyrics-container", _scrollSpeed);
        else
            await JSRuntime.InvokeVoidAsync("window.scrollingFunctions.stopAutoScroll");
        StateHasChanged();
    }

    private async Task ResetScroll()
    {
        await JSRuntime.InvokeVoidAsync("eval", "document.getElementById('genius-lyrics-container').scrollTop = 0");
        if (_isScrolling) await ToggleScroll(true);
    }

    private async Task OnSpeedChanged(int newSpeed)
    {
        _scrollSpeed = newSpeed;
        if (_isScrolling)
            await JSRuntime.InvokeVoidAsync("window.scrollingFunctions.startAutoScroll", "genius-lyrics-container", _scrollSpeed);
    }

    private void OnFontScaleChanged(double newScale)
    {
        _fontScale = newScale;
        StateHasChanged();
    }
}