using Microsoft.AspNetCore.Mvc;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;

namespace SetlistManager.Api.Controllers;

public class StatisticsController : BaseController
{
    private readonly ISongService _songService;
    
    public StatisticsController(ISongService songService)
    {
        _songService = songService;
    }

    [HttpGet]
    public async Task<ActionResult<List<SongUsageStatModel>>> Get([FromQuery] StatsRequest request)
    {
        if (request.Subject is not StatsSubject.Song)
            return BadRequest("Only Song subject is currently supported");

        return Ok(await _songService.GetStatisticsAsync(request));
    }
}