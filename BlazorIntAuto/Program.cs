using Auth0.AspNetCore.Authentication;
using BlazorIntAuto.Common.Extentions;
using BlazorIntAuto.Common.Interfaces;
using BlazorIntAuto.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedForHeaderName = "HeaderNamUsedByProxy_X-Forwarded-For_Header";
    options.ForwardedProtoHeaderName = "HeaderNamUsedByProxy_X-Forwarded-Proto_Header";
});

var connectionString = builder.Configuration["MongoDB:Uri"];

if (connectionString == null)
{
    Console.WriteLine("You must set your 'MONGODB_URI' environment variable. To learn how to set it, see https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#set-your-connection-string");
    Environment.Exit(0);
}
builder.Services.AddDbContext<MongoDbContext>(options =>
    options.UseMongoDB(connectionString, "Gamify"));

builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerAppService, PlayerAppService>();
builder.Services.AddScoped<ITokenValidatedService, TokenValidatedService>();

builder.Services
    .AddAuth0WebAppAuthentication(options =>
    {
        var domain = builder.Configuration["Auth0:Domain"];
        var clientId = builder.Configuration["Auth0:ClientId"];
        if (string.IsNullOrEmpty(domain) || string.IsNullOrEmpty(clientId))
        {
            Console.WriteLine("You must set your Auth0 domain and client ID. To learn how to set it, see https://auth0.com/docs/quickstart/webapp/aspnet-core");
            Environment.Exit(0);
        }
        
        options.Domain = domain;
        options.ClientId = clientId;

        options.OpenIdConnectEvents = new OpenIdConnectEvents
        {
            OnTokenValidated = async (context) =>
            {
                var tokenValidatedService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatedService>();
                await tokenValidatedService.HandleValidatedToken(context);
            }
        };
    });

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var mongoDbContext = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
    mongoDbContext.Database.EnsureCreated();
}


app.Use((context, next) =>
{
    context.Request.Scheme = "https";
    return next(context);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseForwardedHeaders();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    // app.UseHsts();
    app.UseForwardedHeaders();
}

// app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();


app.MapGet("/api/players", async (MongoDbContext mongoDbContext) =>
{
    var players = await mongoDbContext.Players.ToListAsync();
    return Results.Ok(players);
});

app.MapPut("/api/players/{id}", async (IPlayerAppService playerService, PlayerDto player) =>
{
    await playerService.Update(player);
    return Results.Ok();
});


app.MapGet("/api/players/{id}", async (IPlayerAppService playerService, string id) =>
{
        var player = await playerService.GetById(id);
        return Results.Ok(player);
});

app.MapGet("/api/players/me", async (HttpContext httpContext, IPlayerAppService playerService) =>
{
    if(!httpContext.User.Identity.IsAuthenticated)
    {
        return Results.Unauthorized();
    }
    var user = httpContext.User;
    string id = httpContext.User.GetUserId();
    var player = await playerService.GetById(id);
    return Results.Ok(player);
});

app.MapGet("/api/players/qrcode/{id}", async (IPlayerAppService playerService, string id) =>
{
    var qrcode = await playerService.GetQrCode(id);
    return Results.Ok(qrcode);
});


app.MapGet("/Account/Login", async (HttpContext httpContext, string returnUrl = "/") =>
{
    var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
        .WithRedirectUri(returnUrl)
        .Build();

    await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
});

app.MapGet("/Account/Logout", async (HttpContext httpContext, string returnUrl = "/") =>
{
    var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
        .WithRedirectUri(returnUrl)
        .Build();

    await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BlazorIntAuto.Client._Imports).Assembly);

app.Run();
