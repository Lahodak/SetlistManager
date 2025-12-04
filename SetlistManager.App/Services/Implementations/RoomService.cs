using Blazored.LocalStorage;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using MudBlazor.Extensions;
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

    public async Task<RoomModel?> CreateRoomAsync(RoomCreateModel createRoomModel)
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

    public async Task<PagedResponse<RoomModel>?> GetPublicActiveRoomsAsync(PagedRequest request)
    {
        using var httpClient = _httpClientFactory.CreateClient();
        await ConfigureHttpClientAsync(httpClient);

        UriBuilder uri = new(_apiOptions.Value.RoomsEndpoint)
        {
            Query = new QueryBuilder
            {
                { "PageSize", request.PageSize.ToString() },
                { "PageIndex", request.PageIndex.ToString() },
                { "Query", request.Query ?? string.Empty }
            }.ToString()
        };

        var response = await httpClient.GetAsync(uri.ToString());
        var jsonResponse = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(jsonResponse))
            return default;

        PagedResponse<RoomModel>? result;

        try
        {
            result = JsonConvert.DeserializeObject<PagedResponse<RoomModel>?>(jsonResponse);
        }
        catch
        {
            return null;
        }

        return result;
    }
}