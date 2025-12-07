using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using SetlistManager.App.Options;
using SetlistManager.Common.Models;

namespace SetlistManager.App.Services.Implementations;

public class SetlistService : ISetlistService
{
    private const string _setlistByIdSuffix = "/";

    private readonly IApiService _apiService;
    private readonly IOptions<SetlistManagerApiOptions> _apiOptions;

    public SetlistService(IApiService apiService, IOptions<SetlistManagerApiOptions> apiOptions)
    {
        _apiOptions = apiOptions;
        _apiService = apiService;
    }

    public async Task SaveSetlistAsync(SetlistModel setlistModel) 
        => await _apiService.PostAsync(_apiOptions.Value.SetlistsEndpoint, setlistModel);

    public async Task<SetlistModel?> GetSetlistById(int id) 
        => await _apiService.GetAsync<SetlistModel>(_apiOptions.Value.SetlistsEndpoint + _setlistByIdSuffix + id);

    public async Task<PagedResponse<SetlistModel>?> GetAllSetlistsAsync(PagedRequest request)
    {
        UriBuilder uri = new(_apiOptions.Value.SetlistsEndpoint)
        {
            Query = new QueryBuilder
            {
                { "PageSize", request.PageSize.ToString() },
                { "PageIndex", request.PageIndex.ToString() },
                { "Query", request.Query ?? string.Empty }
            }.ToString()
        };


        return await _apiService.GetAsync<PagedResponse<SetlistModel>?>(uri.ToString());
    }

    public async Task EditSetlist(SetlistModel setlistModel)
        => await _apiService.PutAsync(_apiOptions.Value.SetlistsEndpoint + _setlistByIdSuffix + setlistModel.Id, setlistModel);

    public async Task<bool> TryDeleteSetlistAsync(int id)
        => await _apiService.TryDeleteAsync(_apiOptions.Value.SetlistsEndpoint + _setlistByIdSuffix + id);
}