using System.Net.Http.Json;
using BlazorIntAuto.Common.Interfaces;

public class PlayerAppService(HttpClient httpClient) : IPlayerAppService
{
    public async Task<PlayerDto> GetPlayerById(string id)
    {
        var player = await httpClient.GetFromJsonAsync<PlayerDto>($"https://localhost:7275/api/players/{id}");
        if (player == null)
        {
            throw new Exception("Player not found");
        }
        return player;
    }
}
