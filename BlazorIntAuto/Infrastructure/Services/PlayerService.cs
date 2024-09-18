using BlazorIntAuto.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

public interface IPlayerService
{
    Task<Player> GetPlayerById(string id);
    Task<Player?> GetPlayerByAuthId(string authId);

    Task<Player> UpdatePlayer(Player player);

    Task<Player[]> GetPlayers();
    Task<Player> AddPlayer(Player player);

}

public class PlayerService(MongoDbContext dbContext) : IPlayerService
{
    public async Task<Player> AddPlayer(Player player)
    {
        dbContext.Players.Add(player);
        await dbContext.SaveChangesAsync();
        return player;
    }

    public async Task<Player?> GetPlayerByAuthId(string authId)
    {
        return await dbContext.Players.FirstOrDefaultAsync(x => x.AuthId == authId);
    }

    public async Task<Player> GetPlayerById(string id)
    {
        ObjectId.TryParse(id, out var objectId);
        var player = await dbContext.Players.FirstOrDefaultAsync(x => x.Id == objectId);
        if (player == null)
        {
            throw new Exception("Player not found");
        }
        return player;
    }

    public Task<Player[]> GetPlayers()
    {
        throw new NotImplementedException();
    }

    public async Task<Player> UpdatePlayer(Player player)
    {
        var playerToUpdate = await dbContext.Players.FirstOrDefaultAsync(x => x.Id == player.Id);
        if (playerToUpdate == null)
        {
            throw new Exception("Player not found");
        }
        playerToUpdate = player;
        await dbContext.SaveChangesAsync();
        return playerToUpdate;
    }
}