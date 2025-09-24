using Microsoft.AspNetCore.Mvc;
using SetlistManager.Api.Models;
using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;
using SetlistManager.Business.Mappers;
using SetlistManager.Business.Services;

namespace SetlistManager.Api.Controllers;

[Route("api/songs")]

public partial class SongsController : BaseController
{
    private readonly ISongService _songService;
    private readonly ILanguageService _languageService;

    public SongsController(ISongService songService, ILanguageService languageService)
    {
        _songService = songService;
        _languageService = languageService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SongModel>>> GetSongs([FromQuery] string? name)
    {
        IEnumerable<Song?> songs;

        if (string.IsNullOrWhiteSpace(name))
        {
            songs = await _songService.GetSongsAsync();
        }
        else
        {
            songs = await _songService.GetSongByNameAsync(name);
        }

        if (songs is null)        
            return NotFound();        

        var songModels = new List<SongModel>();
        var languages = await _languageService.GetAvailableLanguagesAsync();

        foreach (var song in songs)
        {
            var songModel = song!.ToModel();
            var language = languages.First(x => x.Id == songModel.LanguageId);
            songModel.Language = language;
            songModels.Add(songModel);
        }

        return Ok(songModels);
    }

    [HttpPost("bulk")]
    public async Task AddSongs(AddSongsModel addSongs)
    {
        foreach(var song in addSongs.Songs)
        {
            await _songService.UploadSongAsync(new()
            {
                Language = await _languageService.GetLanguageByIdAsync(song.Language.Id),
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
        await _songService.UploadSongAsync(new()
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
            UpdatedBy = addSong.UpdatedBy,
            Language = await _languageService.GetLanguageByIdAsync(addSong.Language.Id),
        });
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SongModel>> GetSongById(int id)
    {
        var song = await _songService.GetSongByIdAsync(id);

        if (song is null)
        {
            return NotFound();
        }

        var songModel = song.ToModel();

        songModel.Language = (await _languageService.GetLanguageByIdAsync(songModel.LanguageId)).ToModel();

        return Ok(songModel);
    }
}