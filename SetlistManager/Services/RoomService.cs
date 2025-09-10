using SetlistManager.Common.Models;

namespace SetlistManager.Services;

public class RoomService
{
    private const string _roomsEndpointPath = "https://localhost:7143/api/rooms";

    private readonly ApiService _apiService;

    public RoomService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task JoinRoomAsync(JoinRoomModel joinRoomModel)
        => await _apiService.GetAsync<RoomModel>(_roomsEndpointPath, joinRoomModel);
}