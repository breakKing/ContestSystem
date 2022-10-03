using ContestSystem.GatewayApi.Auth.Constants;
using ContestSystem.GatewayApi.Auth.Models;

namespace ContestSystem.GatewayApi.Auth.Services;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(
        LoginCredentials credentials,
        CancellationToken ct = default);

    Task<SignUpResult> SignUpAsync(
        SignUpData data,
        CancellationToken ct = default
    );
}

public class AuthService : IAuthService
{   
    private readonly IHttpClientFactory _factory;

    public AuthService(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    public async Task<LoginResult> LoginAsync(
        LoginCredentials credentials,
        CancellationToken ct = default)
    {
        var client = _factory.CreateClient(AuthHttpClientParameters.ClientName);
        return new LoginResult();
    }

    public async Task<SignUpResult> SignUpAsync(
        SignUpData data,
        CancellationToken ct = default)
    {
        var client = _factory.CreateClient(AuthHttpClientParameters.ClientName);
        return new SignUpResult();
    }
}
