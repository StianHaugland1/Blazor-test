using System.Security.Claims;
using Auth0.AspNetCore.Authentication;
using BlazorIntAuto.Common.Interfaces;
using BlazorIntAuto.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
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

builder.Services
    .AddAuth0WebAppAuthentication(options =>
    {
        options.Domain = builder.Configuration["Auth0:Domain"];
        options.ClientId = builder.Configuration["Auth0:ClientId"];

        options.OpenIdConnectEvents = new OpenIdConnectEvents
        {
            OnTokenValidated = async (context) =>
            {
                var authId = context.SecurityToken.Claims.SingleOrDefault(claim => claim.Type == "sub")?.Value;
                var nickname = context.SecurityToken.Claims.SingleOrDefault(claim => claim.Type == "nickname")?.Value;
                var name = context.SecurityToken.Claims.SingleOrDefault(claim => claim.Type == "name")?.Value;

                if (string.IsNullOrWhiteSpace(authId))
                {
                    context.Fail("Invalid token");
                }
                var dbContext = context.HttpContext.RequestServices.GetRequiredService<MongoDbContext>();

                var player = dbContext.Players.FirstOrDefault(x => x.AuthId == authId);
                if (player == null)
                {
                    player = new Player
                    {
                        Id = ObjectId.GenerateNewId(),
                        AuthId = authId!,
                        Nickname = nickname ?? "Player",
                        Name = name ?? "Name not found",
                        Wins = 0,
                        Losses = 0,
                        TotalMatches = 0,
                        Rating = 1500
                    };
                    dbContext.Players.Add(player);
                    await dbContext.SaveChangesAsync();
                }
                if(context?.Principal?.Identity is not null)
                {
                    var claimsIdentity = (ClaimsIdentity)context.Principal.Identity;
                    claimsIdentity.AddClaim(new Claim("db_id", player.Id.ToString()));
                    context.Principal = new ClaimsPrincipal(claimsIdentity);
                }
            }
        };
    });
builder.Services.AddScoped<AuthenticationStateProvider, PersistingAuthenticationStateProvider>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IPlayerAppService, PlayerAppService>();

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

app.MapGet("/test", async (MongoDbContext mongoDbContext) =>
{
    // mongoDbContext.Users.Add(new User { Name = "Stian", Order = "2" });
    // await mongoDbContext.SaveChangesAsync();
    // var user = await mongoDbContext.Users.FirstOrDefaultAsync();
    // return Results.Ok(user);
    return Results.Ok();
});

app.MapGet("/api/players", async (MongoDbContext mongoDbContext) =>
{
    var players = await mongoDbContext.Players.ToListAsync();
    return Results.Ok(players);
});


app.MapGet("/api/players/{id}", async (IPlayerAppService playerService, string id) =>
{
        var player = await playerService.GetPlayerById(id);
        return Results.Ok(player);
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
