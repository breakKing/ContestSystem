namespace ContestSystem.GatewayApi.Common.Interfaces;

public interface IIdsHasher
{
    string EncodeUserId(long userId);
    long DecodeUserId(string hash);
}
