using Microsoft.AspNetCore.SignalR;
using SetlistManager.Business.Services;
using SetlistManager.Common.Models;
using System.Security.Claims;

namespace SetlistManager.Api.Hubs;

public class RoomHub : Hub
{
    private const string _clientUpdateDataMethod = "UpdateData";
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
        if (Context.User?.Identity is null || !Context.User.Identity.IsAuthenticated)
        {
            return null;
        }

        var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        var currentUser = await _userService.GetUserEntityByIdAsync(userId)
            ?? throw new HubException("Couldn't find user");
        
        var roomModel = await _roomsService.JoinRoomAsync(joinRoomModel, currentUser) 
            ?? throw new HubException($"Couldn't find Room {joinRoomModel.RoomCode}");
        
        await Groups.AddToGroupAsync(Context.ConnectionId, roomModel.Id.ToString());

        await Clients.Group(roomModel.Id.ToString()).SendAsync(_clientUpdateDataMethod, roomModel);

        return roomModel;
    }

    public async Task LeaveRoomAsync(string roomId) 
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync(_clientUpdateDataMethod, Context.ConnectionId);
    }

    public async Task ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel)
    {
        await _roomsService.ChangeCurrentSongAsync(changeCurrentSongModel);
        var roomModel = await _roomsService.GetRoomByIdAsync(changeCurrentSongModel.RoomId!.Value);
        await Clients.Group(changeCurrentSongModel.RoomId!.Value.ToString()).SendAsync(_clientUpdateDataMethod, roomModel);
    }
}