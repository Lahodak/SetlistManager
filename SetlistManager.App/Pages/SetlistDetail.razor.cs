using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class SetlistDetail
{
    [Parameter]
    public int SetlistId { get; set; }

    [Inject]
    public required ISetlistService SetlistService { get; set; }

    private SetlistModel? _setlist = new();

    protected override async Task OnInitializedAsync()
    {
        _setlist = await SetlistService.GetSetlistById(SetlistId)!;
    }
}