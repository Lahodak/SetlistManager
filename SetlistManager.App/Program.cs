using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SetlistManager.App.Services;
using SetlistManager.App;
using SetlistManager.App.Services.Implementations;
using SetlistManager.App.Services.Extentions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddHttpClient();

builder.Services.AddServices();

builder.Services.AddLogging();

builder.Services.AddBlazoredLocalStorageAsSingleton(); 

await builder.Build().RunAsync();