using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SetlistManager;
using SetlistManager.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<SongService>();
builder.Services.AddSingleton<SongsDB>();
builder.Services.AddSingleton<SetlistService>();
builder.Services.AddScoped<LyricsService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddBlazoredLocalStorageAsSingleton(); 
await builder.Build().RunAsync();