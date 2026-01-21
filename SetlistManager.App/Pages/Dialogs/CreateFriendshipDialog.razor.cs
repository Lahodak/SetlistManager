using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages.Dialogs;

public partial class CreateFriendshipDialog
{
    [CascadingParameter]
    public required IMudDialogInstance MudDialog { get; set; }
    
    [Inject]
    public required IUserService UserService { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }

    private string _searchQuery = string.Empty;
    private List<UserViewModel>? _searchResults;
    private HashSet<int> _sentRequests = new();
    private bool _isSearching;
    private bool _hasSearched;

    private async Task SearchUsers()
    {
        if (string.IsNullOrWhiteSpace(_searchQuery))
            return;

        _isSearching = true;
        _hasSearched = true;

        try
        {
            var request = new PagedRequest
            {
                PageSize = 10,
                PageIndex = 0,
                Query = _searchQuery
            };

            var result = await UserService.GetPagedUsersAsync(request);
            var userId = await UserService.GetCurrentUserIdAsync();
            
            if(result?.Items is null)
            {
                _searchResults = [];
                return;
            }

            _searchResults = result?.Items?
                .Where(u => u.Id != userId)
                .ToList();
        }
        finally
        {
            _isSearching = false;
        }
    }

    private async Task OnSearchKeyUp(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
        {
            await SearchUsers();
        }
    }

    private async Task SendFriendRequest(UserViewModel user)
    {
        var friendshipRequest = new FriendshipRequestModel
        {
            RecieverId = user.Id
        };

        await UserService.HandleFriendshipRequestAsync(friendshipRequest);

        _sentRequests.Add(user.Id);        
        MudDialog.Close(Snackbar.Add($"Friend request sent to {user.UserName}!", Severity.Success));
    }

    private void Cancel() => MudDialog.Cancel();
}