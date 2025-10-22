using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SetlistManager.App.Services;
using SetlistManager.App;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddHttpClient();

builder.Services.AddSingleton<SongService>();
builder.Services.AddSingleton<SongsDB>();
builder.Services.AddScoped<SetlistService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<ApiService>();
builder.Services.AddScoped<LanguageService>();
builder.Services.AddScoped<InstrumentService>();
builder.Services.AddScoped<ArtistService>();
builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<GeniusService>();
builder.Services.AddLogging();

builder.Services.AddBlazoredLocalStorageAsSingleton(); 

await builder.Build().RunAsync();