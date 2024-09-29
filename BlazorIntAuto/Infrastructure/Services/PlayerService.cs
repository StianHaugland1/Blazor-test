using BlazorIntAuto.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using QRCoder;

public interface IPlayerService
{
    Task<Player> GetById(string id);
    Task<Player?> GetPlayerByAuthId(string authId);

    Task<Player> Update(Player player);

    Task<Player[]> Get();
    Task<Player> AddPlayer(Player player);

    Task<QrCodeDto> GetQrCode(string id);

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

    public async Task<Player> GetById(string id)
    {
        ObjectId.TryParse(id, out var objectId);
        var player = await dbContext.Players.FirstOrDefaultAsync(x => x.Id == objectId);
        if (player == null)
        {
            throw new Exception("Player not found");
        }
        return player;
    }

    public Task<Player[]> Get()
    {
        throw new NotImplementedException();
    }

    public async Task<Player> Update(Player player)
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

    public Task<QrCodeDto> GetQrCode(string id)
    {
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(id, QRCodeGenerator.ECCLevel.Q))
        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
        {
            byte[] qrCodeImage = qrCode.GetGraphic(20);

            return Task.FromResult(new QrCodeDto
            {
                QrCode = Convert.ToBase64String(qrCodeImage)
            });
        }
    }
}