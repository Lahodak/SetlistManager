using SetlistManager.App.Services;
using SetlistManager.App.Services.Implementations;

namespace SetlistManager.App.Extentions;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IApiService, ApiService>();
        services.AddScoped<ISongService, SongService>();
        services.AddScoped<ISetlistService, SetlistService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IInstrumentService, InstrumentService>();
        services.AddScoped<IArtistService, ArtistService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IGeniusService, GeniusService>();
        return services;
    }
}