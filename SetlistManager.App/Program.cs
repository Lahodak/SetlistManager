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
builder.Services.AddSingleton<SetlistService>();
builder.Services.AddScoped<LyricsService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<ApiService>();
builder.Services.AddSingleton<LanguageService>();
builder.Services.AddTransient<LyricsMarkupService>();
builder.Services.AddScoped<InstrumentService>();

builder.Services.AddBlazoredLocalStorageAsSingleton(); 

await builder.Build().RunAsync();