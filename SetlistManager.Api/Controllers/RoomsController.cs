using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;

namespace SetlistManager.Api.Controllers;

public class RoomsController : BaseController
{
    private readonly IRoomsService _roomsService;

    public RoomsController(IRoomsService roomsService)
    {
        _roomsService = roomsService;
    }

    [HttpPost]
    public async Task<ActionResult<RoomModel>> CreateRoomAsync(RoomCreateModel roomCreateModel)
    {
        return Ok(await _roomsService.CreateRoomAsync(roomCreateModel));
    }

    [HttpGet("{roomId:int}")]
    public async Task<ActionResult<RoomModel>> GetRoomByIdAsync(int roomId)
    {
        return Ok(await _roomsService.GetRoomByIdAsync(roomId));
    }

    [HttpGet("{roomCode}")]
    public async Task<ActionResult<RoomModel>> GetRoomByCodeAsync(string roomCode)
    {
        return Ok(await _roomsService.GetRoomByCodeAsync(roomCode));
    }

    [HttpGet]
    public async Task<ActionResult<PagedResponse<RoomModel>>> GetPublicActiveRoomsAsync([FromQuery] PagedRequest request)
    {
        return Ok(await _roomsService.GetPublicActiveRoomsAsync(request));
    }
}