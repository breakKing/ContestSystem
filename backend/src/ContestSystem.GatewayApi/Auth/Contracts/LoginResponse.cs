using ContestSystem.GatewayApi.Common.Interfaces;

namespace ContestSystem.GatewayApi.Auth.Contracts;

public class LoginResponse : ISuccessResponse
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public string AccessToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
}
