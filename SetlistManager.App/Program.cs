using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SetlistManager.App;
using SetlistManager.App.Extentions;
using SetlistManager.App.Options;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services
    .ConfigureOptions(builder.Configuration)
    .AddMudServices()
    .AddHttpClient()
    .AddServices()
    .AddLogging()
    .AddBlazoredLocalStorageAsSingleton();

await builder.Build().RunAsync();