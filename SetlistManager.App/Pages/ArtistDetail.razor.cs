using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class ArtistDetail
{
    [Parameter]
    public int ArtistId { get; set; }
    [Inject]
    public required IArtistService ArtistService { get; set; }

    private ArtistModel? _artist;

    protected override async Task OnInitializedAsync()
    {
        _artist = await ArtistService.GetArtistByIdAsync(ArtistId)!;
    }
}