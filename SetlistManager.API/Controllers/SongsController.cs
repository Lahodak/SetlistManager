using Microsoft.AspNetCore.Mvc;
using SetlistManager.API.Data;
using SetlistManager.API;
using SetlistManager.API.Models;
using SetlistManager.API.Data.Entities;

using SetlistManager.Common.Models;

namespace SetlistManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public partial class SongsController : ControllerBase
{
    private readonly ISongsDB _songsDB;

    public SongsController(ISongsDB songsDB)
    {
        _songsDB = songsDB;
    }

    [HttpGet]
    public async Task<IEnumerable<SongModel>> GetSongCollection()
    {
        var songs = await _songsDB.GetSongsAsync();
        List<SongModel> songModels = [];

        foreach (var song in songs)
        {
            songModels.Add(song.ToModel());

        }
        return songModels;        
    }

    [HttpPost]
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

    [HttpGet("{songId:int}")]
    public async Task<ActionResult<SongModel>> GetSongById(int songId)
    {
        var song = await _songsDB.GetSongByIdAsync(songId);
        if (song is null)
        {
            return NotFound();
        }
        return song.ToModel();
    }

    [HttpGet("{songName}")]
    public async Task<ActionResult<IEnumerable<SongModel>>> GetSongByName(string songName)
    {
        var songs = await _songsDB.GetSongByNameAsync(songName);
        if(songs is null)
        {
            return NotFound();
        }
        List<SongModel> songModels = [];

        foreach (var song in songs)
        {
            songModels.Add(song!.ToModel());
        }

        return songModels;
    }
}