using ContestSystem.GatewayApi.Common.Interfaces;
using HashidsNet;

namespace ContestSystem.GatewayApi.Common.Services;

public class IdsHasher : IIdsHasher
{
    private readonly Hashids _hashids;

    public IdsHasher()
    {
        _hashids = new Hashids("ContestSystem", 5);
    }

    public long DecodeUserId(string hash)
    {
        var rawIds = _hashids.DecodeLong(hash);

        if (rawIds.Length == 0)
        {
            return default;
        }

        return rawIds[0];
    }

    public string EncodeUserId(long userId)
    {
        return _hashids.EncodeLong(userId);
    }
}
