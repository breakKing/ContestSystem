using ContestSystem.GatewayApi.Auth.Constants;
using ContestSystem.GatewayApi.Auth.Contracts;
using ContestSystem.GatewayApi.Auth.Models;
using ContestSystem.GatewayApi.Auth.Services;
using ContestSystem.GatewayApi.Common.Interfaces;

namespace ContestSystem.GatewayApi.Auth.Endpoints;

public class LoginEndpoint: Endpoint<LoginRequest, LoginResponse>
{
    private readonly IAuthService _authService;
    private readonly IMapper<LoginRequest, LoginCredentials> _requestMapper;
    private readonly IMapper<LoginResult, LoginResponse> _responseMapper;

    public LoginEndpoint(
        IAuthService authService,
        IMapper<LoginRequest, LoginCredentials> requestMapper,
        IMapper<LoginResult, LoginResponse> responseMapper)
    {
        _authService = authService;
        _requestMapper = requestMapper;
        _responseMapper = responseMapper;
    }

    public override void Configure()
    {
        Post(AuthEndpoints.Login);
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        
    }
}
