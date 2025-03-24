using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TrelloClone.Client;
using TrelloClone.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient
builder.Services.AddScoped(sp => 
{
    var httpClient = new HttpClient 
    { 
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress.Replace("5007", "5022").TrimEnd('/') + "/") 
    };
    return httpClient;
});

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ISignalRService, SignalRService>();
builder.Services.AddScoped<ILoggingService, LoggingService>();

await builder.Build().RunAsync();
