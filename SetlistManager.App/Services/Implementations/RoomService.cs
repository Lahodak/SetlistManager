using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class RoomService : IRoomService
{
    private readonly IOptions<SetlistManagerApiOptions> _apiOptions;
    private readonly IApiService _apiService;

    public RoomService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions;
        _apiService = apiService;
    }

    public async Task JoinRoomAsync(JoinRoomModel joinRoomModel)
        => await _apiService.GetAsync<RoomModel>(_apiOptions.Value.RoomsEndpoint);

    public async Task<RoomModel?> CreateRoomAsync(CreateRoomModel createRoomModel)
        => await _apiService.PostAsync<CreateRoomModel, RoomModel> (_apiOptions.Value.RoomsEndpoint, createRoomModel);

    public async Task<RoomModel?> GetRoomByCodeAsync(string roomCode)
        => await _apiService.GetAsync<RoomModel>($"{_apiOptions.Value.RoomsEndpoint}/{roomCode}");

    public async Task<List<RoomModel>?> GetPublicActiveRoomsAsync()
        => await _apiService.GetAsync<List<RoomModel>?>(_apiOptions.Value.RoomsEndpoint);
}