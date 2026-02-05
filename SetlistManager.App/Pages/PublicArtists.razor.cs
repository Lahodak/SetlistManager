using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class PublicArtists
{
    [Inject]
    public required IArtistService ArtistService { get; set; }

    [Inject]
    public required IUserService UserService { get; set; }

    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private MudTable<ArtistModel> table = new();
    private PagedRequest pageState = new()
    {
        ContentType = ContentType.Public
    };
    private string? searchString;
    private int _userId;
    private bool _loading = true;
    private HashSet<int> _userArtistIds = [];

    protected override async Task OnInitializedAsync()
    {
        _userId = (await UserService.GetCurrentUserIdAsync()).Value;
        await LoadUserArtists();
    }

    private async Task LoadUserArtists()
    {
        var userArtistsRequest = new PagedRequest
        {
            ContentType = ContentType.Private,
            PageSize = int.MaxValue
        };

        var userArtists = await ArtistService.GetArtistsAsync(userArtistsRequest);
        if (userArtists?.Items != null)
        {
            _userArtistIds = userArtists.Items.Select(a => a.Id).ToHashSet();
        }
    }

    private async Task<TableData<ArtistModel>?> ServerReload(TableState state, CancellationToken token)
    {
        _loading = true;

        pageState.Query = searchString;
        pageState.PageIndex = state.Page;
        pageState.PageSize = state.PageSize;

        var response = await ArtistService.GetArtistsAsync(pageState);
        _loading = false;

        if (response?.Items is null)
            return null;

        IEnumerable<ArtistModel>? filtered = response.Items;

        filtered = state.SortLabel switch
        {
            "nick_field" => filtered.OrderByDirection(state.SortDirection, a => a.Nick),
            "songs_field" => filtered.OrderByDirection(state.SortDirection, a => a.Songs?.Count ?? 0),
            _ => filtered
        };

        return new TableData<ArtistModel>
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

    private async Task AddToLibrary(ArtistModel artist)
    {
        var result = await ArtistService.TryGiveAccessToUserAsync(artist.Id, _userId);
        if (result)
        {
            Snackbar.Add($"'{artist.Nick}' added to your library.", Severity.Success);
            _userArtistIds.Add(artist.Id);
        }
        else
        {
            Snackbar.Add($"Failed to add '{artist.Nick}' to your library.", Severity.Error);
        }
    }
}