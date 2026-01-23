using SetlistManager.Common.Models;
using MudBlazor;

namespace SetlistManager.App.Pages;

public partial class Home
{
    private StatsRange _selectedRange = StatsRange.Week;
    private List<string> MostUsedLabels = [];
    private List<string> MostAddedLabels = [];
    private List<ChartSeries> MostUsedSeries = [];
    private List<ChartSeries> MostAddedSeries = [];
    private List<LatestSongStatModel>? LatestSongs;

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
            MostUsedLabels = [];
            MostUsedSeries = [];

            StateHasChanged();
            return;
        }

        MostUsedLabels = response.Items
            .Select(x => x.Name)
            .ToList();

        MostUsedSeries = new()
        {
            new ChartSeries
            {
                Name = "Usage Count",
                Data = response.Items
                    .Select(x => (double)x.UsageCount)
                    .ToArray()
            }
        };
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
            MostAddedLabels = [];
            MostAddedSeries = [];

            StateHasChanged();
            return;
        }

        MostAddedLabels = response.Items
            .Select(x => x.Name)
            .ToList();

        MostAddedSeries = new()
        {
            new ChartSeries
            {
                Name = "Added Count",
                Data = response.Items
                    .Select(x => (double)x.UsageCount)
                    .ToArray()
            }
        };
        StateHasChanged();
    }

    private async Task LoadLatestPublicAsync()
    {
        var response = await SongService.GetLatestPublicSongsAsync(new PagedRequest
        {
            PageIndex = 0,
            PageSize = 5
        });
        LatestSongs = response?.Items;
        StateHasChanged();
    }
}