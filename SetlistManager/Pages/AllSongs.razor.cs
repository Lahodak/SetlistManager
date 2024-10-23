using Microsoft.AspNetCore.Components;
using SetlistManager.Models;
using SetlistManager.Services;

namespace SetlistManager.Pages;

public partial class AllSongs
{
    private readonly List<Song>_songCollection = [];
    [Inject]
    public SongsDB SongsDatabase { get; set; }
    protected override void OnInitialized()
    {
        _songCollection.AddRange(SongsDatabase.GetSongCollection());            
    }
}