using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SetlistManager.Api.Extensions;
using SetlistManager.Api.Hubs;
using SetlistManager.Api.Middleware;
using SetlistManager.Business.Extensions;
using SetlistManager.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddHttpClient()
    .AddExceptionHandler<GlobalExceptionHandlingMiddleware>()
    .AddProblemDetails()
    .AddApiServices()
    .AddBusinessServices()
    .AddDatabase(builder.Configuration)
    .ConfigureOptions(builder.Configuration)
    .AddIdentityConfiguration()
    .AddSignalRConfiguration()
    .AddCorsConfiguration()
    .AddAuthenticationConfiguration(builder.Configuration);

var app = builder.Build();

app.UseSwagger()
    .UseSwaggerUI();

app.UseCors("AllowAllPolicy")
    .UseHttpsRedirection()
    .UseAuthentication();

//using (var scope = app.Services.CreateScope())
//{
//    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    await dbContext.Database.MigrateAsync();
//}

app.UseAuthorization();
app.UseResponseCompression();

app.UseExceptionHandler();

app.MapControllers();

app.MapHub<RoomHub>("/hubs/room");

await app.RunAsync();

await app.RunAsync();