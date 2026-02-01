using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SetlistManager.Api.Extentions;
using SetlistManager.Api.Hubs;
using SetlistManager.Api.Middleware;
using SetlistManager.Business.Extentions;
using SetlistManager.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

builder.Services.AddExceptionHandler<GlobalExceptionHandlingMiddleware>();
builder.Services.AddProblemDetails();

builder.Services.AddApiServices()
    .AddBusinessServices();

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SetlistManagerDB")
        ?? throw new InvalidOperationException("Connection string 'SetlistManagerDB' not found.");
    options.UseSqlServer(connectionString);
});

builder.Services
    .ConfigureOptions(builder.Configuration)
    .AddIdentityConfiguration()
    .AddSignalRConfiguration()
    .AddCorsConfiguration()
    .AddAuthenticationConfiguration(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseAuthorization();

app.UseResponseCompression();

app.UseExceptionHandler();

app.MapControllers();

app.MapHub<RoomHub>("/hubs/room");

await app.RunAsync();