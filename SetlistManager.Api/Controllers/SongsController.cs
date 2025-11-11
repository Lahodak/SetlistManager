using Microsoft.AspNetCore.Mvc;
using SetlistManager.Api.Models;
using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;
using SetlistManager.Business.Mappers;
using SetlistManager.Business.Services;

namespace SetlistManager.Api.Controllers;

[Route("api/songs")]

public class SongsController : BaseController
{
    private readonly ISongService _songService;
    private readonly ILanguageService _languageService;
    private readonly IArtistService _artistService;

    public SongsController(ISongService songService, ILanguageService languageService, IArtistService artistService)
    {
        _songService = songService;
        _languageService = languageService;
        _artistService = artistService;
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

    [HttpPost]
    public async Task AddSong(SongModel addSong)
    {
        await _songService.UploadSongAsync(new()
        {
            Name = addSong.Name,
            Artist = await _artistService.GetArtistByIdAsync(addSong.Artist.Id),
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