using Microsoft.AspNetCore.Components;
using SetlistManager.App.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Pages;

public partial class AllSongs
{   
    [Inject]
    public required SongsDB SongsDatabase { get; set; }

    private readonly List<SongModel> _songCollection = [];
    private string input = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _songCollection.AddRange(await SongsDatabase.GetSongCollection());            
    }
}