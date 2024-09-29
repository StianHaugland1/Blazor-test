namespace BlazorIntAuto.Common.Interfaces;
public interface IPlayerAppService
{
    Task<PlayerDto> GetById(string id);
    Task <PlayerDto[]> Get();
    Task Update(PlayerDto player);
    Task<QrCodeDto> GetQrCode(string id);
}

public record QrCodeDto
{
    public string QrCode { get; set; } = "";
}