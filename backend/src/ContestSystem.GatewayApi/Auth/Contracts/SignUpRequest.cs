namespace ContestSystem.GatewayApi.Auth.Contracts;

public class SignUpRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? PasswordRepeat { get; set; }
}
