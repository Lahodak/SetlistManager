using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

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
    
    public async Task CheckForData()
    {
        if (_songsDB.Count != 0)        
            _songsDB.Clear();
        
        _songsDB.AddRange(await _songService.GetAllSongsAsync() ?? []);
    }
}