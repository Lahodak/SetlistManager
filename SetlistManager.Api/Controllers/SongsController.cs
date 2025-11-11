using Microsoft.AspNetCore.Mvc;
using SetlistManager.Api.Models;
using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;
using SetlistManager.Business.Mappers;
using SetlistManager.Business.Services;
using SetlistManager.Api.Services;

namespace SetlistManager.Api.Controllers;

[Route("api/songs")]

public class SongsController : BaseController
{
    private readonly ISongService _songService;
    private readonly ILanguageService _languageService;
    private readonly IArtistService _artistService;
    private readonly ICurrentUserContext _userContext;

    public SongsController(ISongService songService, ILanguageService languageService, IArtistService artistService, ICurrentUserContext userContext)
    {
        _songService = songService;
        _languageService = languageService;
        _artistService = artistService;
        _userContext = userContext;
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
    public async Task AddSong(SongCreateModel createModel)
    {
        var userId = _userContext.GetCurrentUserId();

        await _songService.UploadSongAsync(createModel, userId!.Value);
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