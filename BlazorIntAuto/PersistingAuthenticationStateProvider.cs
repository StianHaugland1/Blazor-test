// using System.Diagnostics;
// using Auth0BlazorDemo.Client.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;


public class PersistingAuthenticationStateProvider : ServerAuthenticationStateProvider, IDisposable
{
    private Task<AuthenticationState>? _authenticationStateTask;
    private readonly PersistentComponentState _state;
    private readonly PersistingComponentStateSubscription _subscription;
    private readonly IdentityOptions _options;
    private readonly IPlayerService _playerService;

    public PersistingAuthenticationStateProvider(
        PersistentComponentState persistentComponentState,
        IOptions<IdentityOptions> optionsAccessor,
        IPlayerService playerService
        )
    {
        _options = optionsAccessor.Value;
        _state = persistentComponentState;
        AuthenticationStateChanged += OnAuthenticationStateChanged;
        _subscription = _state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
        _playerService = playerService;

    }

    private async Task OnPersistingAsync()
    {
        if (_authenticationStateTask is null)
        {
            throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
        }

        var authenticationState = await _authenticationStateTask;
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var userId = principal.FindFirst(_options.ClaimsIdentity.UserIdClaimType)?.Value;
            var name = principal.FindFirst("name")?.Value;
            var p = await _playerService.GetPlayerById(userId);
            if (userId != null && name != null)
            {
                _state.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    UserId = userId,
                    Name = p.Id.ToString(),
                    SomeThing = "WHAAATEVER"
                });
            }
        }
    }



    private void OnAuthenticationStateChanged(Task<AuthenticationState> authenticationStateTask)
    {
        _authenticationStateTask = authenticationStateTask;
    }

    public void Dispose()
    {
        _authenticationStateTask?.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
        _subscription.Dispose();
    }
}

public class UserInfo
{
    public required string UserId { get; set; }
    public required string Name { get; set; }
    public required string SomeThing { get; set; }
}