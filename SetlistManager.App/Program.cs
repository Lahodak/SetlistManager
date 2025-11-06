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

builder.Services.AddMudServices();
builder.Services.AddHttpClient();

builder.Services.Configure<SetlistManagerApiOptions>(builder.Configuration.GetSection(SetlistManagerApiOptions.SectionName))
    .Configure<GeniusOptions>(builder.Configuration.GetSection(GeniusOptions.SectionName));

builder.Services.AddServices();

builder.Services.AddLogging();

builder.Services.AddBlazoredLocalStorageAsSingleton(); 

await builder.Build().RunAsync();