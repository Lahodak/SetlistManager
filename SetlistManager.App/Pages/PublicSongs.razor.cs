using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class PublicSongs
{
    [Inject]
    public required ISongService SongService { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private MudTable<SongModel> table = new();
    private ContentPagedRequest pageState = new()
    {
        ContentType = ContentType.Public
    };
    private string? searchString;
    private int _userId;
    private bool _loading = true;
    private HashSet<int> _userSongIds = [];

    protected override async Task OnInitializedAsync()
    {
        _userId = (await UserService.GetCurrentUserIdAsync()).Value;
        await LoadUserSongs();
    }

    private async Task LoadUserSongs()
    {
        var userSongsRequest = new ContentPagedRequest
        {
            PageSize = int.MaxValue
        };

        var userSongs = await SongService.GetSongsAsync(userSongsRequest);
        if (userSongs?.Items != null)
        {
            _userSongIds = userSongs.Items
                .Select(s => s.Id)
                .ToHashSet();
        }
    }

    private async Task<TableData<SongModel>?> ServerReload(TableState state, CancellationToken token)
    {
        _loading = true;

        pageState.Query = searchString;
        pageState.PageIndex = state.Page;
        pageState.PageSize = state.PageSize;

        var response = await SongService.GetSongsAsync(pageState);

        _loading = false;

        if (response?.Items is null)
            return null;

        IEnumerable<SongModel>? filtered = response.Items;

        filtered = state.SortLabel switch
        {
            "title_field" => filtered.OrderByDirection(state.SortDirection, s => s.Name),
            "artist_field" => filtered.OrderByDirection(state.SortDirection, s => s.Artist),
            _ => filtered
        };

        return new TableData<SongModel>
        {
            TotalItems = response.TotalCount,
            Items = filtered
        };
    }

    private void OnSearch(string text)
    {
        searchString = text;
        table.ReloadServerData();
    }

    private async Task AddToLibrary(SongModel song)
    {
        var result = await SongService.TryGiveAccessToUserAsync(_userId, song.Id);

        if(result)
        {
            Snackbar.Add($"'{song.Name}' added to your library.", Severity.Success);
            _userSongIds.Add(song.Id);
        }
        else
        {
            Snackbar.Add($"Failed to add '{song.Name}' to your library.", Severity.Error);
        }
    }
}