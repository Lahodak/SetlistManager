using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SetlistManager;
using SetlistManager.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddHttpClient();
builder.Services.AddSingleton<SongService>();
builder.Services.AddSingleton<SongsDB>();
builder.Services.AddScoped<LyricsService>();
await builder.Build().RunAsync();