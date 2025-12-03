using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Api.Services;
using SetlistManager.Business.Services;

namespace SetlistManager. Api.Controllers;

[Route("api/rooms")]

public class RoomsController : BaseController
{
    private readonly IRoomsService _roomsService;
    private readonly ICurrentUserContext _userContext;

    public RoomsController(IRoomsService roomsService, ICurrentUserContext userContext)
    {
        _roomsService = roomsService;
        _userContext = userContext;
    }

    [HttpPost]
    public async Task<ActionResult<RoomModel>> CreateRoomAsync(RoomCreateModel roomCreateModel)
    {
        var userId = _userContext.GetCurrentUserId();

        if (userId is null)
            return Unauthorized();

        var roomModel = await _roomsService.CreateRoomAsync(roomCreateModel, userId.Value);        

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

        if(rooms.Count == 0)
            return NotFound("No active rooms found");

        return Ok(rooms);
    }
}