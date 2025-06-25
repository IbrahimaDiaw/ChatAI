using ChatAIClient;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddSyncfusionBlazor();
// Register the Syncfusion license key
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(builder.Configuration["Syncfusion:LicenseKey"]);

builder.Services.AddSingleton<HubConnection>(sp =>
{
    return new HubConnectionBuilder()
        .WithUrl($"{builder.Configuration["Serveur:Url"]}")
        .WithAutomaticReconnect()
        .Build();
});

await builder.Build().RunAsync();
