using BlazorIntAuto.Common.Interfaces;

public class PlayerAppService(IPlayerService playerService) : IPlayerAppService
{
    public async Task<PlayerDto> GetById(string id)
    {
        var player = await playerService.GetById(id);
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

    public Task<PlayerDto[]> Get()
    {
        throw new NotImplementedException();
    }

    public async Task Update(PlayerDto player)
    {
        var playerToUpdate = await playerService.GetById(player.Id);
        playerToUpdate.Name = player.Name;
        playerToUpdate.Nickname = player.Nickname;
        playerToUpdate.Emoji = player.Emoji;
        playerToUpdate.Wins = player.Wins;
        playerToUpdate.Losses = player.Losses;
        playerToUpdate.TotalMatches = player.TotalMatches;
        playerToUpdate.Rating = player.Rating;
        await playerService.Update(playerToUpdate);
    }
}
