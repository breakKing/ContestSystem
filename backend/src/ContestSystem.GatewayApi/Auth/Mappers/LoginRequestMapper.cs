using ContestSystem.GatewayApi.Auth.Contracts;
using ContestSystem.GatewayApi.Auth.Models;
using ContestSystem.GatewayApi.Common.Interfaces;

namespace ContestSystem.GatewayApi.Auth.Mappers;

public class LoginRequestMapper : IMapper<LoginRequest, LoginCredentials>
{
    public LoginCredentials Convert(LoginRequest src)
    {
        return new LoginCredentials
        {
            Login = src.Login,
            Password = src.Password
        };
    }
}
