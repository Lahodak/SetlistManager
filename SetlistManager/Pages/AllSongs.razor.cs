using Microsoft.AspNetCore.Components;
using SetlistManager.Common.Models;
using SetlistManager.Services;

namespace SetlistManager.Pages;

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