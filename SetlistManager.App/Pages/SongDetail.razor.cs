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
            await JSRuntime.InvokeVoidAsync("geniusEmbed.loadEmbed",
                _lyricsData.SongId,
                _lyricsData.Title,
                _lyricsData.Artist,
                _lyricsData.Url);
        }
    }
}