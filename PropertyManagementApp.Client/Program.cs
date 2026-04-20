using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PropertyManagementApp.Client;
using PropertyManagementApp.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// point to the API - in production you'd read this from config
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5001/")
});

builder.Services.AddMudServices();
builder.Services.AddScoped<NotificationService>();

await builder.Build().RunAsync();
