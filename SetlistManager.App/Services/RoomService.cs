using SetlistManager.Common.Models;

namespace SetlistManager.App.Services;

public class RoomService
{
    private readonly string _roomsEndpointPath;

    private readonly ApiService _apiService;

    public RoomService(ApiService apiService, IConfiguration configuration)
    {
        _roomsEndpointPath = configuration["SetlistManager.Api:RoomsEndpoint"]!;
        _apiService = apiService;
    }

    public async Task JoinRoomAsync(JoinRoomModel joinRoomModel)
        => await _apiService.GetAsync<RoomModel>(_roomsEndpointPath);
}