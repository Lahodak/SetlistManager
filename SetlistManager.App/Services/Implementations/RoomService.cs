using Blazored.LocalStorage;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class RoomService : IRoomService
{
    private readonly IOptions<SetlistManagerApiOptions> _apiOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocalStorageService _localStorage;
    public HubConnection HubConnection { get; }
    public event Action<RoomModel>? RoomUpdated;


    public RoomService(IHttpClientFactory httpClientFactory, IOptions<SetlistManagerApiOptions> apiOptions, ILocalStorageService localStorage)
    {
        _httpClientFactory = httpClientFactory;
        _apiOptions = apiOptions;
        _localStorage = localStorage;

        HubConnection = new HubConnectionBuilder()
            .WithUrl(_apiOptions.Value.RoomHubEndpoint, options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    return await _localStorage.GetItemAsync<string>("authToken");
                };
            })
            .WithAutomaticReconnect()
            .Build();

        HubConnection.On<RoomModel>("UpdateData", (room) =>
        {
            RoomUpdated?.Invoke(room);
        });
    }

    private async Task ConfigureHttpClientAsync(HttpClient httpClient)
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
    
    public async Task<RoomModel?> JoinRoomAsync(JoinRoomModel joinRoomModel)
    {
        if (HubConnection.State == HubConnectionState.Disconnected)
            await HubConnection.StartAsync();

        try
        {
            var room = await HubConnection.InvokeAsync<RoomModel>("JoinRoomAsync", joinRoomModel);

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
            var room = await HubConnection.InvokeAsync<RoomModel>("ChangeCurrentSongAsync", changeCurrentSongModel);
            return room;
        }
        catch
        {
            return null;
        }
    }

    public async Task<RoomModel?> CreateRoomAsync(CreateRoomModel createRoomModel)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        await ConfigureHttpClientAsync(httpClient);
        string jsonData;

        try
        {
            jsonData = JsonConvert.SerializeObject(createRoomModel);
        }
        catch
        {
            return default;
        }

        var content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(_apiOptions.Value.RoomsEndpoint, content);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(jsonResponse))
            return default;

        RoomModel? result;

        try
        {
            result = JsonConvert.DeserializeObject<RoomModel>(jsonResponse);
        }
        catch
        {
            return null;
        }

        return result;
    }

    public async Task<List<RoomModel>?> GetPublicActiveRoomsAsync()
    {

        using var httpClient = _httpClientFactory.CreateClient();
        await ConfigureHttpClientAsync(httpClient);

        var response = await httpClient.GetAsync(_apiOptions.Value.RoomsEndpoint);
        response.EnsureSuccessStatusCode();
        var jsonResponse = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(jsonResponse))
            return default;

        List<RoomModel>? result;

        try
        {
            result = JsonConvert.DeserializeObject<List<RoomModel>>(jsonResponse);
        }
        catch
        {
            return null;
        }

        return result;
    }
}