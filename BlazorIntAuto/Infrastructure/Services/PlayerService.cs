using BlazorIntAuto.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;

public interface IPlayerService
{
    Task<Player> GetPlayerById(string id);
}

public class PlayerService(MongoDbContext dbContext) : IPlayerService
{
    public async Task<Player> GetPlayerById(string id)
    {
        var player = await dbContext.Players.FirstOrDefaultAsync(x => x.AuthId == id);
        ObjectId.TryParse(id, out var objectId);
        // await dbContext.Players.Find(s => s.Id == id);
        if (player == null)
        {
            throw new Exception("Player not found");
        }
        return player;
    }
}