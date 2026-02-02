using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;
using System.Net.Http.Json;

namespace SetlistManager.App.Services.Implementations;

public class RoomService : IRoomService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IUserService _userService;
    private readonly IApiService _apiService;
    private readonly SetlistManagerApiOptions _apiOptions;
    public event Action<RoomModel>? RoomUpdated;
    private const string _joinRoomMethod = "JoinRoomAsync";
    private const string _updateDataMethod = "UpdateData";
    private const string _changeCurrentSongMethod = "ChangeCurrentSongAsync";
    private const string _authBearerKey = "Bearer";
    public HubConnection HubConnection { get; }

    public RoomService(IHttpClientFactory httpClientFactory, IOptions<SetlistManagerApiOptions> apiOptions, IApiService apiService, IUserService userService)
    {
        _httpClientFactory = httpClientFactory;
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
        _userService = userService;
    }

    private async Task ConfigureHttpClientAsync(HttpClient httpClient)
    {
        var token = await _userService.GetUserTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(_authBearerKey, token);
        }
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
        var httpClient = _httpClientFactory.CreateClient();
        await ConfigureHttpClientAsync(httpClient);
  
        var response = await httpClient.PostAsJsonAsync(_apiOptions.RoomsEndpoint, createModel);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<RoomModel>();
    }

    public async Task<PagedResponse<RoomModel>> GetPublicActiveRoomsAsync(PagedRequest request)
    {
        UriBuilder uri = new(_apiOptions.RoomsEndpoint)
        {
            Query = new QueryBuilder
            {
                { nameof(request.PageSize), request.PageSize.ToString() },
                { nameof(request.PageIndex), request.PageIndex.ToString() },
                { nameof(request.Query), request.Query ?? string.Empty }
            }.ToString()
        };

        var response = await _apiService.GetAsync<PagedResponse<RoomModel>>(uri.ToString());

        return response!;
    }
}