namespace BlazorIntAuto.Common.Interfaces;
public interface IPlayerAppService
{
    Task<PlayerDto> GetPlayerById(string id);
    // Task<Player[]> GetPlayers();
    // Task<Player> AddPlayer(Player player);
    // Task<Player> UpdatePlayer(Player player);
    // Task<Player> DeletePlayer(int id);
}