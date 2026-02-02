using SetlistManager.App.Options;
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
        services.AddScoped<IQRService, QRService>();
        return services;
    }

    public static IServiceCollection ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SetlistManagerApiOptions>(configuration.GetSection(SetlistManagerApiOptions.SectionName))
                .Configure<GeniusOptions>(configuration.GetSection(GeniusOptions.SectionName));
        return services;
    }
}