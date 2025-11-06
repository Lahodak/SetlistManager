using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SetlistManager.Api.Services;
using SetlistManager.Business.Services.Implementations;
using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Api.Hubs;

public class RoomHub : Hub
{
    private readonly RoomsService _roomsService;
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserContext _currentUserContext;

    public RoomHub(RoomsService roomsService, UserManager<User> userManager, ICurrentUserContext currentUserContext)
    {
        _roomsService = roomsService;
        _userManager = userManager;
        _currentUserContext = currentUserContext;
    }

    public async Task<RoomModel> JoinRoomAsync(string roomCode)
    {
        var userId = _currentUserContext.GetCurrentUserId();
        var user = await _userManager.FindByIdAsync(userId.ToString()!);

        if (user is null)
            throw new HubException("Couldn't find user");

        var roomModel = await _roomsService.JoinRoomAsync(new JoinRoomModel { RoomCode = roomCode }, user);

        if (roomModel is null)
            throw new HubException($"Couldn't find Room {roomCode}");

        await Groups.AddToGroupAsync(Context.ConnectionId, roomModel.Id.ToString());

        await Clients.Group(roomModel.Id.ToString()).SendAsync("UpdateData", Context.ConnectionId);

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