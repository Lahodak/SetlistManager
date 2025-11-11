using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SetlistManager.Api.Services;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;
using System.Security.Claims;

namespace SetlistManager.Api.Hubs;

public class RoomHub : Hub
{
    private readonly IRoomsService _roomsService;
    private readonly IUserService _userService;

    public RoomHub(IRoomsService roomsService, IUserService userService)
    {
        _roomsService = roomsService;
        _userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        if (Context.User is null || Context.User.Identity is null || !Context.User.Identity.IsAuthenticated)
        {
            Context.Abort();
            return;
        }

        var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
        {
            Context.Abort();
            return;
        }

        var currentUser = await _userService.GetUserEntityByIdAsync(userId);

        if (currentUser is null)
        {
            Context.Abort();            
        }

        await base.OnConnectedAsync();
    }

    public async Task<RoomModel?> JoinRoomAsync(JoinRoomModel joinRoomModel)
    {
        if (Context.User is null || Context.User.Identity is null || !Context.User.Identity.IsAuthenticated)
        {
            return null;
        }

        var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        var currentUser = await _userService.GetUserEntityByIdAsync(userId);

        if (currentUser is null)
            throw new HubException("Couldn't find user");

        var roomModel = await _roomsService.JoinRoomAsync(joinRoomModel, currentUser);

        if (roomModel is null)
            throw new HubException($"Couldn't find Room {joinRoomModel.RoomCode}");

        await Groups.AddToGroupAsync(Context.ConnectionId, roomModel.Id.ToString());

        await Clients.Group(roomModel.Id.ToString()).SendAsync("UpdateData", roomModel);

        return roomModel;
    }

    public async Task LeaveRoomAsync(string roomId) 
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("UpdateData", Context.ConnectionId);
    }

    public async Task ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel)
    {
        await _roomsService.ChangeCurrentSongAsync(changeCurrentSongModel);
        var roomModel = await _roomsService.GetRoomByIdAsync(changeCurrentSongModel.RoomId);
        await Clients.Group(changeCurrentSongModel.RoomId.ToString()).SendAsync("UpdateData", roomModel);
    }
}