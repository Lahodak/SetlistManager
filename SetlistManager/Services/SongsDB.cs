using SetlistManager.Models;
namespace SetlistManager.Services;
public class SongsDB
{
    private readonly List<Song> _songsDB = [];
    public void AddSongsToSongsDB(List<Song> LoadedSongs) => _songsDB.AddRange(LoadedSongs);
    public void RemoveSongsFromDB(Song SongToRemove) => _songsDB.Remove(SongToRemove);
    public List<Song> GetSongCollection() => _songsDB;
    public Song GetSong(int id) => _songsDB.Where(song => song.SongID == id).FirstOrDefault();
    public int GetCount() => _songsDB.Count;
}