namespace BlazorIntAuto.Common.Interfaces;
public interface IPlayerAppService
{
    Task<PlayerDto> GetPlayerById(string id);
    Task <PlayerDto[]> GetPlayers();
    Task UpdatePlayer(PlayerDto player);
}