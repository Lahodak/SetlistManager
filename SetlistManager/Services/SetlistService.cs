namespace SetlistManager.Services;

public class SetlistService
{
    private const string _pathSetlists = "https://localhost:7143/api/Setlists";
    private IHttpClientFactory _httpClientFactory;
    public SetlistService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public async Task PushSetlistToApi()
    {

    }
}