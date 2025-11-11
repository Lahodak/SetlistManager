using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager. Api.Services;
using SetlistManager.Data.Entities;
using SetlistManager.Business.Services;
namespace SetlistManager. Api.Controllers;

[Route("api/rooms")]

public class RoomsController : BaseController
{
    private readonly UserManager<User> _userManager;
    private readonly IRoomsService _roomsService;
    private readonly ICurrentUserContext _currentUserContext;

    public RoomsController(UserManager<User> userManager, IRoomsService roomsService, ICurrentUserContext currentUserContext)
    {
        _userManager = userManager;
        _roomsService = roomsService;
        _currentUserContext = currentUserContext;
    }

    [HttpPost]
    public async Task<ActionResult<RoomModel>> CreateRoomAsync(CreateRoomModel roomCreateModel)
    {
        var userId = (int)_currentUserContext.GetCurrentUserId()!;
        var roomModel = await _roomsService.CreateRoomAsync(roomCreateModel, userId);        

        return Ok(roomModel);
    }

    [HttpGet("{roomId:int}")]
    public async Task<ActionResult<RoomModel>> GetRoomByIdAsync(int roomId)
    {
        var roomModel = await _roomsService.GetRoomByIdAsync(roomId);
        
        if (roomModel is null)
            return NotFound("Room not found");
        
        return Ok(roomModel);
    }

    [HttpGet("{roomCode}")]
    public async Task<ActionResult<RoomModel>> GetRoomByCodeAsync(string roomCode)
    {
        var roomModel = await _roomsService.GetRoomByCodeAsync(roomCode);
        
        if (roomModel is null)
            return NotFound("Room not found");
        
        return Ok(roomModel);
    }

    [HttpGet]
    public async Task<ActionResult<List<RoomModel>>> GetAllActiveRoomsAsync()
    {
        var rooms = await _roomsService.GetPublicActiveRoomsAsync();
        return Ok(rooms);
    }
}