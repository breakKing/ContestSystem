using ContestSystem.GatewayApi.Auth.Contracts;
using ContestSystem.GatewayApi.Auth.Models;
using ContestSystem.GatewayApi.Common.Interfaces;

namespace ContestSystem.GatewayApi.Auth.Mappers;

public class SignUpResponseMapper : IMapper<SignUpResult, SignUpResponse>
{
    private readonly IIdsHasher _idsHasher;

    public SignUpResponseMapper(IIdsHasher idsHasher)
    {
        _idsHasher = idsHasher;
    }

    public SignUpResponse Convert(SignUpResult src)
    {
        return new SignUpResponse
        {
            Success = src.Success,
            Errors = src.Errors,
            UserId = _idsHasher.EncodeUserId(src.UserId)
        };
    }
}
