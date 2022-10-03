namespace ContestSystem.GatewayApi.Auth.Models;

public class LoginResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
