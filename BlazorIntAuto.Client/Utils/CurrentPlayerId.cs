using BlazorIntAuto.Common.Extentions;
using Microsoft.AspNetCore.Components.Authorization;

interface ICurrentPlayerService
{
    public Task<string> GetCurrentPlayerId();
}
public class CurrentPlayerService(AuthenticationStateProvider authenticationStateProvider) : ICurrentPlayerService
{
    public async Task<string> GetCurrentPlayerId()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            return user.GetUserId();
        }

        throw new Exception("User not authenticated");
    }
}