using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using SetlistManager.App.Extensions;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class RoomService : IRoomService
{
    private readonly IUserService _userService;
    private readonly IApiService _apiService;
    private readonly SetlistManagerApiOptions _apiOptions;

    public event Action<RoomModel>? RoomUpdated;

    private const string _joinRoomMethod = "JoinRoomAsync";
    private const string _updateDataMethod = "UpdateData";
    private const string _changeCurrentSongMethod = "ChangeCurrentSongAsync";

    public HubConnection HubConnection { get; }

    public RoomService(IOptions<SetlistManagerApiOptions> apiOptions, IApiService apiService, IUserService userService)
    {
        _apiOptions = apiOptions.Value;
        _userService = userService;
        _apiService = apiService;

        HubConnection = new HubConnectionBuilder()
            .WithUrl(_apiOptions.RoomHubEndpoint, options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    return await _userService.GetUserTokenAsync();
                };
            })
            .WithAutomaticReconnect()
            .Build();

        HubConnection.On<RoomModel>(_updateDataMethod, (room) =>
        {
            RoomUpdated?.Invoke(room);
        });
    }

    public async Task<RoomModel?> JoinRoomAsync(JoinRoomModel joinRoomModel)
    {
        if (HubConnection.State == HubConnectionState.Disconnected)
            await HubConnection.StartAsync();

        try
        {
            var room = await HubConnection.InvokeAsync<RoomModel>(_joinRoomMethod, joinRoomModel);
            return room;
        }
        catch
        {
            return null;
        }
    }

    public async Task<RoomModel?> ChangeCurrentSongAsync(ChangeCurrentSongModel changeCurrentSongModel)
    {
        if (HubConnection.State == HubConnectionState.Disconnected)
            await HubConnection.StartAsync();

        try
        {
            var room = await HubConnection.InvokeAsync<RoomModel>(_changeCurrentSongMethod, changeCurrentSongModel);
            return room;
        }
        catch
        {
            return null;
        }
    }

    public async Task<RoomModel?> CreateRoomAsync(RoomCreateModel createModel)
    {
        var response = await _apiService.PostAsync<RoomCreateModel, RoomModel>(_apiOptions.RoomsEndpoint, createModel);
        return response;
    }

    public async Task<PagedResponse<RoomModel>> GetPublicActiveRoomsAsync(PagedRequest request)
    {
        var uri = request.ToUri(_apiOptions.RoomsEndpoint);
        var response = await _apiService.GetAsync<PagedResponse<RoomModel>>(uri);
        return response!;
    }
}