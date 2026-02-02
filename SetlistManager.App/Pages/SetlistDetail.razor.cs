using Microsoft.AspNetCore.Components;
using MudBlazor;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class SetlistDetail
{
    [Parameter]
    public int SetlistId { get; set; }

    [Inject]
    public required ISetlistService SetlistService { get; set; }
    [Inject]
    public required ISnackbar Snackbar { get; set; }
    [Inject]
    public required NavigationManager NavigationManager { get; set; }

    private SetlistModel? _setlist;

    protected override async Task OnInitializedAsync()
    {
        _setlist = await SetlistService.GetSetlistById(SetlistId)!;

        if(_setlist is null)
        {
            Snackbar.Add("Setlist not found or you don't have access to it.", Severity.Error);
        }
    }
}