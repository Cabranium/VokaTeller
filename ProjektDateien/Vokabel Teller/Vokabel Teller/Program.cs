using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Vokabel_Teller;
using Blazor.IndexedDB; 
using Microsoft.JSInterop;
using Vokabel_Teller.OwnClasses;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Registriere IndexedDbFactory als Singleton
builder.Services.AddSingleton<IIndexedDbFactory, IndexedDbFactory>();
await builder.Build().RunAsync();


