using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using MongoDB.Bson;

public interface ITokenValidatedService
{
    Task HandleValidatedToken(TokenValidatedContext context);
}

public class TokenValidatedService(IPlayerService playerService) : ITokenValidatedService
{
    public async Task HandleValidatedToken(TokenValidatedContext context)
    {
        var authId = context.SecurityToken.Claims.SingleOrDefault(claim => claim.Type == "sub")?.Value;
        var nickname = context.SecurityToken.Claims.SingleOrDefault(claim => claim.Type == "nickname")?.Value;
        var name = context.SecurityToken.Claims.SingleOrDefault(claim => claim.Type == "name")?.Value;

        if (string.IsNullOrWhiteSpace(authId))
        {
            context.Fail("Invalid token");
        }
        var player = await playerService.GetPlayerByAuthId(authId!);
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
            await playerService.AddPlayer(player);
        }
        
        if (context?.Principal?.Identity is not null)
        {
            var claimsIdentity = (ClaimsIdentity)context.Principal.Identity;
            claimsIdentity.AddClaim(new Claim("db_id", player.Id.ToString()));
            context.Principal = new ClaimsPrincipal(claimsIdentity);
        }
    }
}