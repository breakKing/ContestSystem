namespace ContestSystem.GatewayApi.Common.Interfaces;

public interface ISuccessResponse
{
    bool Success { get; set; }
    List<string> Errors { get; set; }
}
