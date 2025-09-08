using Microsoft.AspNetCore.Mvc;
using SetlistManager.API.Data;
using SetlistManager.API.Models;
using SetlistManager.Common.Models;

namespace SetlistManager.API.Controllers;
[Route("api/songs")]
public partial class SongsController : BaseController
{
    private readonly ISongsDB _songsDB;

    public SongsController(ISongsDB songsDB)
    {
        _songsDB = songsDB;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SongModel>>> GetSongs([FromQuery] string? name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            var songs = await _songsDB.GetSongByNameAsync(name);

            if (songs is null || !songs.Any())
            {
                return NotFound();
            }

            return songs.Select(s => s!.ToModel()).ToList();
        }

        var allSongs = await _songsDB.GetSongsAsync();
        return allSongs.Select(s => s.ToModel()).ToList();
    }


    [HttpPost("bulk")]
    public async Task AddSongs(AddSongsModel addSongs)
    {
        foreach(var song in addSongs.Songs)
        {
            await _songsDB.UploadSong(new()
            {
                Name = song.Name,
                Artist = song.Artist,
                TabsURL = song.TabsURL,
                AudioURL = song.AudioURL,
                LanguageId = song.LanguageId,
                Key = song.Key,
                Tuning = song.Tuning,
                BPM = song.BPM,
                CreatedAt = song.CreatedAt,
                UpdatedAt = song.UpdatedAt,
                UpdatedBy = song.UpdatedBy
            });
        }
    }

    [HttpPost]
    public async Task AddSong(SongModel addSong)
    {
        await _songsDB.UploadSong(new()
        {
            Name = addSong.Name,
            Artist = addSong.Artist,
            TabsURL = addSong.TabsURL,
            AudioURL = addSong.AudioURL,
            LanguageId = addSong.LanguageId,
            Key = addSong.Key,
            Tuning = addSong.Tuning,
            BPM = addSong.BPM,
            CreatedAt = addSong.CreatedAt,
            UpdatedAt = addSong.UpdatedAt,
            UpdatedBy = addSong.UpdatedBy
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SongModel>> GetSongById(int id)
    {
        var song = await _songsDB.GetSongByIdAsync(id);

        if (song is null)
        {
            return NotFound();
        }

        return song.ToModel();
    }
}