using System.Net.Http.Json;
using BlazorIntAuto.Common.Interfaces;

public class PlayerClientService(HttpClient httpClient) : IPlayerAppService
{
    public async Task<PlayerDto> GetPlayerById(string id)
    {
        var player = await httpClient.GetFromJsonAsync<PlayerDto>($"/api/players/{id}");
        if (player == null)
        {
            throw new Exception("Player not found");
        }
        return player;
    }

    public async Task<PlayerDto[]> GetPlayers()
    {
        var players = await httpClient.GetFromJsonAsync<PlayerDto[]>($"/api/players");
        if (players == null)
        {
            throw new Exception("Players are null");
        }
        return players;
    }

    public async Task UpdatePlayer(PlayerDto player)
    {
        var responseMessage =  await httpClient.PutAsJsonAsync($"/api/players/{player.Id}", player);

        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new Exception("Failed to update player");
        }
    }
}
