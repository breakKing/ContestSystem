namespace ContestSystem.GatewayApi.Auth.Contracts;

public class SignUpRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordRepeat { get; set; } = string.Empty;
}
