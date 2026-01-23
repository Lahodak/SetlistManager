using SetlistManager.Common.Models;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;

namespace SetlistManager.App.Pages;

public partial class Home
{
    [Inject]
    public required ISongService SongService { get; set; }
    private StatsRange _selectedRange = StatsRange.Week;
    private List<string> _mostUsedLabels = [];
    private List<string> _mostAddedLabels = [];
    private List<ChartSeries> _mostUsedSeries = [];
    private List<ChartSeries> _mostAddedSeries = [];
    private List<LatestSongStatModel>? _latestSongs;

    protected override async Task OnInitializedAsync()
    {
        await LoadDashboardAsync();
    }

    private async Task OnRangeChanged(StatsRange range)
    {
        _selectedRange = range;
        await LoadMostUsedAsync();
    }

    private async Task LoadDashboardAsync()
    {
        await LoadMostUsedAsync();
        await LoadMostAddedAsync();
        await LoadLatestPublicAsync();
    }

    private async Task LoadMostUsedAsync()
    {
        var response = await SongService.GetMostUsedSongsAsync(new StatsPagedRequest
        {
            Range = _selectedRange,
            PageIndex = 0,
            PageSize = 5
        });

        if (response?.Items is null || response.Items.Count == 0)
        {
            _mostUsedLabels = [];
            _mostUsedSeries = [];

            StateHasChanged();
            return;
        }

        _mostUsedLabels = response.Items
            .Select(x => x.Name)
            .ToList();

        _mostUsedSeries =
        [
            new ChartSeries
            {
                Name = "Usage Count",
                Data = response.Items
                    .Select(x => (double)x.UsageCount)
                    .ToArray()
            }
        ];
        StateHasChanged();
    }

    private async Task LoadMostAddedAsync()
    {
        var response = await SongService.GetMostAddedToLibraryAsync(new PagedRequest
        {
            PageIndex = 0,
            PageSize = 5
        });

        if (response?.Items is null || response.Items.Count == 0)
        {
            _mostAddedLabels = [];
            _mostAddedSeries = [];

            StateHasChanged();
            return;
        }

        _mostAddedLabels = response.Items
            .Select(x => x.Name)
            .ToList();

        _mostAddedSeries =
        [
            new ChartSeries
            {
                Name = "Added Count",
                Data = response.Items
                    .Select(x => (double)x.UsageCount)
                    .ToArray()
            }
        ];
        StateHasChanged();
    }

    private async Task LoadLatestPublicAsync()
    {
        var response = await SongService.GetLatestPublicSongsAsync(new PagedRequest
        {
            PageIndex = 0,
            PageSize = 5
        });
        _latestSongs = response?.Items;
        StateHasChanged();
    }
}