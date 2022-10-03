using ContestSystem.GatewayApi.Auth.Constants;
using ContestSystem.GatewayApi.Auth.Contracts;
using ContestSystem.GatewayApi.Auth.Models;
using ContestSystem.GatewayApi.Auth.Services;
using ContestSystem.GatewayApi.Common.Interfaces;

namespace ContestSystem.GatewayApi.Auth.Endpoints;

public class SignUpEndpoint: Endpoint<SignUpRequest, SignUpResponse>
{
    private readonly IAuthService _authService;
    private readonly IMapper<SignUpRequest, SignUpData> _requestMapper;
    private readonly IMapper<SignUpResult, SignUpResponse> _responseMapper;

    public SignUpEndpoint(
        IAuthService authService,
        IMapper<SignUpRequest, SignUpData> requestMapper,
        IMapper<SignUpResult, SignUpResponse> responseMapper)
    {
        _authService = authService;
        _requestMapper = requestMapper;
        _responseMapper = responseMapper;
    }

    public override void Configure()
    {
        Post(AuthEndpoints.SignUp);
        AllowAnonymous();
    }

    public override async Task HandleAsync(SignUpRequest req, CancellationToken ct)
    {
        
    }
}
