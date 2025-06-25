using ChatClient;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
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
    var connection = new HubConnectionBuilder()
        .WithUrl($"{builder.Configuration["Serveur:Url"]}", options =>
        {
            options.SkipNegotiation = false;
            options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets;
        })
        .WithAutomaticReconnect(new[] { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30) })
        .Build();

    return connection;
});

await builder.Build().RunAsync();
