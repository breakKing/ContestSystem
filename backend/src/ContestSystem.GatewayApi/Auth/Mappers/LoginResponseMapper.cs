using ContestSystem.GatewayApi.Auth.Contracts;
using ContestSystem.GatewayApi.Auth.Models;
using ContestSystem.GatewayApi.Common.Interfaces;

namespace ContestSystem.GatewayApi.Auth.Mappers;

public class LoginResponseMapper : IMapper<LoginResult, LoginResponse>
{
    public LoginResponse Convert(LoginResult src)
    {
        return new LoginResponse
        {
            Success = src.Success,
            Errors = src.Errors,
            AccessToken = src.AccessToken,
            ExpiresAt = src.ExpiresAt
        };
    }
}
