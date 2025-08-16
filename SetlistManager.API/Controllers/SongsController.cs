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
        SongModel model;
        var x = await _songsDB.GetSongsAsync();
        List<SongModel> modelList = [];

        foreach (var y in x)
        {
            model = y.ToModel();
            modelList.Add(model);
        }
        return modelList;        
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

    [HttpGet("{SongId}")]
    public async Task<ActionResult<SongModel>> GetSongById(int SongId)
    {
        var song = await _songsDB.GetSongByIdAsync(SongId);
        if (song is null)
        {
            return NotFound();
        }
        return song.ToModel();
    }
}