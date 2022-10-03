namespace ContestSystem.GatewayApi.Auth.Constants;

internal class AuthHttpClientParameters
{
    internal static readonly string ClientName = "AuthClient";
    internal static readonly TimeSpan FirstRetryTimeSpan = TimeSpan.FromSeconds(1);
    internal static readonly int RetryAttempts = 5;
}
