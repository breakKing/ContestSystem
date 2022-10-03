namespace ContestSystem.GatewayApi.Auth.Models;

public class SignUpData
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordRepeat { get; set; } = string.Empty;
}
