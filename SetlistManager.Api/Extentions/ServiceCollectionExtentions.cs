using SetlistManager.Api.Services;

namespace SetlistManager. Api.Extentions;

public static class ServiceCollectionExtentions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICurrentUserContext, CurrentUserContext>();

        return services;
    }
}