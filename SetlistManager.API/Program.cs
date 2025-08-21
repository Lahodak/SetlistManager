using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SetlistManager.API;
using SetlistManager.API.Data;
using SetlistManager.API.Data.Entities;
var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

IConfiguration configuration = builder.Configuration;

builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("SongDB")
        ?? throw new ApplicationException("The connection string is null");
    return new SqlConnectionFactory(connectionString);
});

builder.Services.AddScoped<ISongsDB, SongsDB>();
builder.Services.AddScoped<ISetlistsDB, SetlistsDB>();
builder.Services.AddScoped<IRoomsDB, RoomsDB>();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});



builder.Services.AddDbContext<APIDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("SetlistManagerDB")
        ?? throw new InvalidOperationException("Connection string 'SetlistManagerDB' not found.");
    options.UseSqlServer(connectionString);
});

builder.Services.AddIdentity<User, Role>(options =>
                                       options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<APIDbContext>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllPolicy");


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
