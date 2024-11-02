using Microsoft.AspNetCore.Components;
using SetlistManager.Models;
namespace SetlistManager.Services;
public class SongsDB
{
    private readonly SongService _songService;
    private readonly List<Song> _songsDB = [];

    public SongsDB(SongService songService)
    {
        _songService = songService;
    }

    public void RemoveSongsFromDB(Song songToRemove) 
        => _songsDB.Remove(songToRemove);

    public async Task<List<Song>> GetSongCollection()
    {
        await CheckForData();
        return _songsDB;
    }

    public Song? GetSong(int id) 
        => _songsDB.FirstOrDefault(song => song.SongID == id);
    
    public int GetCount() 
        => _songsDB.Count;
    
    private async Task CheckForData()
    {
        if (_songsDB.Count != 0)
            return;
        _songsDB.AddRange(await _songService.FetchSongsFromAPI() ?? []);
    }
}