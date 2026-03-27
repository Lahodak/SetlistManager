using Microsoft.AspNetCore.Mvc;
using SetlistManager.Common.Models;
using SetlistManager.Business.Services;

namespace SetlistManager.Api.Controllers;

/// <summary>
/// Manages live session rooms.
/// </summary>
public class RoomsController : BaseController
{
    private readonly IRoomsService _roomsService;

    public RoomsController(IRoomsService roomsService)
    {
        _roomsService = roomsService;
    }

    /// <summary>Creates a new room with an optional setlist.</summary>
    /// <param name="roomCreateModel">The room creation payload.</param>
    [HttpPost]
    public async Task<ActionResult<RoomModel>> CreateRoomAsync(RoomCreateModel roomCreateModel)
    {
        return Ok(await _roomsService.CreateRoomAsync(roomCreateModel));
    }

    /// <summary>Gets a room by its identifier.</summary>
    /// <param name="roomId">The room identifier.</param>
    [HttpGet("{roomId:int}")]
    public async Task<ActionResult<RoomModel>> GetRoomByIdAsync(int roomId)
    {
        return Ok(await _roomsService.GetRoomByIdAsync(roomId));
    }

    /// <summary>Gets a room by its unique join code.</summary>
    /// <param name="roomCode">The room code.</param>
    [HttpGet("{roomCode}")]
    public async Task<ActionResult<RoomModel>> GetRoomByCodeAsync(string roomCode)
    {
        return Ok(await _roomsService.GetRoomByCodeAsync(roomCode));
    }

    /// <summary>Gets a paginated list of public, active rooms.</summary>
    /// <param name="request">Pagination and search parameters.</param>
    [HttpGet]
    public async Task<ActionResult<PagedResponse<RoomModel>>> GetPublicActiveRoomsAsync([FromQuery] PagedRequest request)
    {
        return Ok(await _roomsService.GetPublicActiveRoomsAsync(request));
    }
}