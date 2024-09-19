namespace BlazorIntAuto.Common.Interfaces;
public interface IPlayerAppService
{
    Task<PlayerDto> GetById(string id);
    Task <PlayerDto[]> Get();
    Task Update(PlayerDto player);
}