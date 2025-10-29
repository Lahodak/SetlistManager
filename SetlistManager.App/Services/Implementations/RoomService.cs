using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class RoomService : IRoomService
{
    private readonly string _roomsEndpointPath;

    private readonly IApiService _apiService;

    public RoomService(IApiService apiService, IConfiguration configuration)
    {
        _roomsEndpointPath = configuration["SetlistManager.Api:RoomsEndpoint"]!;
        _apiService = apiService;
    }

    public async Task JoinRoomAsync(JoinRoomModel joinRoomModel)
        => await _apiService.GetAsync<RoomModel>(_roomsEndpointPath);

    public async Task<RoomModel?> CreateRoomAsync(CreateRoomModel createRoomModel)
        => await _apiService.PostAsync<CreateRoomModel, RoomModel> (_roomsEndpointPath, createRoomModel);

    public async Task<RoomModel?> GetRoomByCodeAsync(string roomCode)
        => await _apiService.GetAsync<RoomModel>($"{_roomsEndpointPath}/{roomCode}");

    public async Task<List<RoomModel>?> GetPublicActiveRoomsAsync()
        => await _apiService.GetAsync<List<RoomModel>?>($"{_roomsEndpointPath}");
}