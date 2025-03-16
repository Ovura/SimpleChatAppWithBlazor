using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SimpleChatAppWithBlazor;
using SimpleChatAppWithBlazor.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

// Register chat service with the server URL
builder.Services.AddScoped<IChatService>(sp =>
    new ChatService("https://localhost:7269/chathub"));

await builder.Build().RunAsync();
