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
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IUserService _userService;
    private User? _currentUser;

    public RoomHub(IRoomsService roomsService, IUserService userService, ICurrentUserContext currentUserContext)
    {
        _roomsService = roomsService;
        _currentUserContext = currentUserContext;
        _userService = userService;
    }

    public override async Task OnConnectedAsync()
    {
        if (!Context.User.Identity.IsAuthenticated)
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

        _currentUser = await _userService.GetUserEntityByIdAsync(userId);

        if (_currentUser is null)
        {
            Context.Abort();            
        }

        await base.OnConnectedAsync();
    }

    public async Task<RoomModel> JoinRoomAsync(JoinRoomModel joinRoomModel)
    {
        if (!Context.User.Identity.IsAuthenticated)
        {
            return null;
        }

        var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        _currentUser = await _userService.GetUserEntityByIdAsync(userId);

        if (_currentUser is null)
            throw new HubException("Couldn't find user");

        var roomModel = await _roomsService.JoinRoomAsync(joinRoomModel, _currentUser);

        if (roomModel is null)
            throw new HubException($"Couldn't find Room {joinRoomModel.RoomCode}");

        await Groups.AddToGroupAsync(Context.ConnectionId, roomModel.Id.ToString());

        //await Clients.Group(roomModel.Id.ToString()).SendAsync("UpdateData", Context.ConnectionId);

        return roomModel;
    }

    public async Task LeaveRoomAsync(string roomId) 
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("UpdateData", Context.ConnectionId);
    }

    public async Task ChangeCurrentSongAsync(string roomId, string songId)
    {
        await Clients.Group(roomId).SendAsync("CurrentSongChanged", songId);
    }
}