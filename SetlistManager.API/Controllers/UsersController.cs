using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SetlistManager.API.Data;
using SetlistManager.API.Data.Entities;
using SetlistManager.API.Services;
using SetlistManager.Common.Models;
using System.Security.Claims;

namespace SetlistManager.API.Controllers;

[Route("api/users")]
public class UsersController : BaseController
{
    private readonly IUserService _userService;
    private readonly ISetlistsService _setlistsService;
    private readonly AppDbContext _appDbContext;
    private readonly ICurrentUserContext _currentUserContext;

    public UsersController(IUserService userService, ISetlistsService setlistsService, AppDbContext appDbContext,
        ICurrentUserContext currentUserContext)
    {
        _userService = userService;
        _setlistsService = setlistsService;
        _appDbContext = appDbContext;
        _currentUserContext = currentUserContext;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(UserModel model)
    {
        await _userService.UpdateUserAsync(model);
        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<UserModel>> GetUser()
    {        
        var userId = _currentUserContext.GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        var user = await _appDbContext.Users
            .Include(x => x.Instrument)
            .Include(x => x.Room)
            .FirstAsync(x => x.Id == userId);

        Instrument userInstrument;

        if(user!.InstrumentId is not null)
        {
            userInstrument = await _appDbContext.Instruments.FirstAsync(x => x.Id == user.InstrumentId);
            user.Instrument = userInstrument;
        }
        
        if (user == null)
            return NotFound();

        return Ok(user.ToModel());
    }

    [HttpGet("{id:int}/setlists")]
    public async Task<ActionResult<List<SetlistModel>>> GetUserSetlists(int id)
    {
        return Ok(await _setlistsService.GetAllSetlistsOfUserAsync(id));
    }
}