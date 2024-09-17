using BlazorIntAuto.Common.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();
// builder.Services.AddScoped<CookieHandler>();    
builder.Services.AddScoped<IPlayerAppService, PlayerAppService>();
// builder.Services.AddHttpClient("base").AddHttpMessageHandler<CookieHandler>();
// builder.Services.AddScoped<IHttpClientFactory>();
// builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("base"));

builder.Services.AddHttpClient();

await builder.Build().RunAsync();
  