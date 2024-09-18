using BlazorIntAuto.Common.Interfaces;

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
}
