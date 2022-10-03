namespace ContestSystem.GatewayApi.Common.Interfaces;

public interface IMapper<TSrc, TDst>
    where TDst: new()
{
    TDst Convert(TSrc src);
}
