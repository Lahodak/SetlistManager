using Microsoft.AspNetCore.Components;
using SetlistManager.Common.Models;
namespace SetlistManager.Services;
public class SongsDB
{
    private readonly SongService _songService;
    private readonly List<SongModel> _songsDB = [];

    public SongsDB(SongService songService)
    {
        _songService = songService;
    }

    public void RemoveSongsFromDB(SongModel songToRemove) 
        => _songsDB.Remove(songToRemove);

    public async Task<List<SongModel>> GetSongCollection()
    {
        await CheckForData();
        return _songsDB;
    }

    public SongModel? GetSong(int id) 
        => _songsDB.FirstOrDefault(song => song.Id == id);
    
    public int GetCount() 
        => _songsDB.Count;
    
    private async Task CheckForData()
    {
        if (_songsDB.Count != 0)
            return;
        _songsDB.AddRange(await _songService.FetchSongsFromAPI() ?? []);
    }
}