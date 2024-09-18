using BlazorIntAuto.Common.Interfaces;
using MongoDB.Bson;

public class PlayerAppService(IPlayerService playerService) : IPlayerAppService
{
    public async Task<PlayerDto> GetPlayerById(string id)
    {
        var player = await playerService.GetPlayerById(id);
        var playerDto = new PlayerDto
        {
            Id = player.Id.ToString(),
            Name = player.Name,
            Nickname = player.Nickname,
            Emoji = player.Emoji,
            Wins = player.Wins,
            Losses = player.Losses,
            TotalMatches = player.TotalMatches,
            Rating = player.Rating
        };
        return playerDto;
    }

    public Task<PlayerDto[]> GetPlayers()
    {
        throw new NotImplementedException();
    }

    public async Task UpdatePlayer(PlayerDto player)
    {
        var playerToUpdate = await playerService.GetPlayerById(player.Id);
        playerToUpdate.Name = player.Name;
        playerToUpdate.Nickname = player.Nickname;
        playerToUpdate.Emoji = player.Emoji;
        playerToUpdate.Wins = player.Wins;
        playerToUpdate.Losses = player.Losses;
        playerToUpdate.TotalMatches = player.TotalMatches;
        playerToUpdate.Rating = player.Rating;
        await playerService.UpdatePlayer(playerToUpdate);
    }
}
