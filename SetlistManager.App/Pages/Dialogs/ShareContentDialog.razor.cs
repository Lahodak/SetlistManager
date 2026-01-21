using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using SetlistManager.App.Models;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages.Dialogs;

public partial class ShareContentDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }
    [Parameter]
    public ShareContentType ContentType { get; set; }
    [Parameter]
    public int ContentId { get; set; }
    [Inject]
    public required ISongService SongService { get; set; }
    [Inject]
    public required ISetlistService SetlistService { get; set; }
    [Inject]
    public required IArtistService ArtistService { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private PagedResponse<FriendModel>? _userFriendships;
    private string _searchQuery = string.Empty;
    private readonly HashSet<int> _sharedWith = [];
    private bool _isSharing;
    private string _contentName = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadFriendships();
        await LoadContentName();
    }

    private async Task LoadFriendships()
    {
        var request = new PagedRequest
        {
            PageSize = 50,
            PageIndex = 0,
            Query = string.Empty
        };

        _userFriendships = await UserService.GetUserFriendshipsAsync(request);
        _userFriendships = await UserService.GetUserFriendshipsAsync(request);
        
        if (_userFriendships?.Items is not null)
        {
            _userFriendships.Items = _userFriendships.Items
                .Where(f => f.State == FriendshipState.Accepted)
                .ToList();
        }    
    }

    private async Task LoadContentName()
    {
        _contentName = ContentType switch
        {
            ShareContentType.Song => (await SongService.GetSongByIdAsync(ContentId))?.Name ?? "Unknown Song",
            ShareContentType.Artist => (await ArtistService.GetArtistByIdAsync(ContentId))?.Nick ?? "Unknown Artist",
            ShareContentType.Setlist => (await SetlistService.GetSetlistById(ContentId))?.Name ?? "Unknown Setlist",
            _ => "Unknown Content"
        };
    }

    private async Task SearchFriends()
    {
        if (string.IsNullOrWhiteSpace(_searchQuery))
            await LoadFriendships();
        else
        {
            var request = new PagedRequest
            {
                PageSize = 50,
                PageIndex = 0,
                Query = _searchQuery
            };

            _userFriendships = await UserService.GetUserFriendshipsAsync(request);
        }
    }

    private async Task OnSearchKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SearchFriends();
        }
    }

    private async Task ShareWithFriend(FriendModel friend)
    {
        _isSharing = true;

        try
        {
            bool success = ContentType switch
            {
                ShareContentType.Song => await SongService.TryGiveAccessToUserAsync(ContentId, friend.Id),
                ShareContentType.Artist => await ArtistService.TryGiveAccessToUserAsync(ContentId, friend.Id),
                ShareContentType.Setlist => await SetlistService.TryGiveAccessToUserAsync(ContentId, friend.Id),
                _ => false
            };

            if (success)
            {
                _sharedWith.Add(friend.Id);
                Snackbar.Add($"Shared {_contentName} with {friend.Username}!", Severity.Success);
            }
            else
            {
                Snackbar.Add($"Failed to share {_contentName} with {friend.Username}", Severity.Error);
            }
        }
        finally
        {
            _isSharing = false;
        }
    }

    private void Cancel() => MudDialog.Cancel();
}