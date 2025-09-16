using Microsoft.Extensions.DependencyInjection;
using SetlistManager.Business.Services; 

namespace SetlistManager.Business.Extentions;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IRoomsService, RoomsService>();
        services.AddScoped<ISongService, SongService>();
        services.AddScoped<ISetlistsService, SetlistsService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISetlistsService, SetlistsService>();
        services.AddScoped<IInstrumentsService, InstrumentsService>();
        services.AddScoped<OrderMappingService>();
        return services;
    }
}