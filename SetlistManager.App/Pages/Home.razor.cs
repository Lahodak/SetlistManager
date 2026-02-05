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
    private List<SongUsageStatModel>? _latestSongs;

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
        var response = await SongService.GetStatisticsAsync(new StatsRequest
        {
            Subject = StatsSubject.Song,
            Metric = StatsMetric.MostUsed,
            Range = _selectedRange,
            Limit = 10
        });

        if (response is null || response.Count == 0)
        {
            _mostUsedLabels = [];
            _mostUsedSeries = [];
            StateHasChanged();
            return;
        }

        _mostUsedLabels = response
            .Select(x => x.Name)
            .ToList();

        _mostUsedSeries =
        [
            new ChartSeries
            {
                Name = "Usage Count",
                Data = response
                    .Select(x => (double)x.UsageCount)
                    .ToArray()
            }
        ];
        StateHasChanged();
    }

    private async Task LoadMostAddedAsync()
    {
        var response = await SongService.GetStatisticsAsync(new StatsRequest
        {
            Subject = StatsSubject.Song,
            Metric = StatsMetric.MostAdded,
            Range = StatsRange.Week,
            Limit = 10
        });

        if (response is null || response.Count == 0)
        {
            _mostAddedLabels = [];
            _mostAddedSeries = [];
            StateHasChanged();
            return;
        }

        _mostAddedLabels = response
            .Select(x => x.Name)
            .ToList();

        _mostAddedSeries =
        [
            new ChartSeries
            {
                Name = "Added Count",
                Data = response
                    .Select(x => (double)x.UsageCount)
                    .ToArray()
            }
        ];
        StateHasChanged();
    }
        
    private async Task LoadLatestPublicAsync()
    {
        var response = await SongService.GetStatisticsAsync(new StatsRequest
        {
            Subject = StatsSubject.Song,
            Metric = StatsMetric.LatestPublic,
            Range = StatsRange.Week,
            Limit = 10
        });

        _latestSongs = response;
        StateHasChanged();
    }
}