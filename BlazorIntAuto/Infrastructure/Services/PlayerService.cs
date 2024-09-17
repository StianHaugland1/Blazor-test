using BlazorIntAuto.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

public class PlayerService(MongoDbContext dbContext) : IPlayerService
{
    public async Task<PlayerDto> GetPlayerById(string id)
    {
        var player = await dbContext.Players.FirstOrDefaultAsync(x => x.AuthId == id);
        if (player == null)
        {
            throw new Exception("Player not found");
        }
        var playerDto = new PlayerDto
        {
            Name = player.Name,
            Nickname = player.Nickname,
            Emoji = player.Emoji,
            AuthId = player.AuthId,
            Wins = player.Wins,
            Losses = player.Losses,
            TotalMatches = player.TotalMatches,
            Rating = player.Rating
        };
        return playerDto;
    }
}