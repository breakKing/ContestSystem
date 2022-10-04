using ContestSystem.GatewayApi.Auth.Contracts;
using ContestSystem.GatewayApi.Auth.Models;
using ContestSystem.GatewayApi.Common.Interfaces;

namespace ContestSystem.GatewayApi.Auth.Mappers;

public class SignUpRequestMapper : IMapper<SignUpRequest, SignUpData>
{
    public SignUpData Convert(SignUpRequest src)
    {
        return new SignUpData
        {
            Username = src.Username ?? string.Empty,
            Password = src.Password ?? string.Empty,
            PasswordRepeat = src.PasswordRepeat ?? string.Empty
        };
    }
}
