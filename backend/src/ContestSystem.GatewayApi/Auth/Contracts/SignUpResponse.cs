using ContestSystem.GatewayApi.Common.Interfaces;

namespace ContestSystem.GatewayApi.Auth.Contracts;

public class SignUpResponse : ISuccessResponse
{
    public bool Success { get; set; }
    public List<string> Errors { get; set;} = new();
    public string UserId { get; set; } = string.Empty;
}
