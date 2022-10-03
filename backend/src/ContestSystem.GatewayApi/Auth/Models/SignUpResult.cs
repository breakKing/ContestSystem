namespace ContestSystem.GatewayApi.Auth.Models;

public class SignUpResult
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public long UserId { get; set; }
}
