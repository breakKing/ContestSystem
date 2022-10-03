using ContestSystem.GatewayApi.Auth.Constants;
using ContestSystem.GatewayApi.Auth.Services;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace ContestSystem.GatewayApi.Auth;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpClient(AuthHttpClientParameters.ClientName, client => 
        {
            client.BaseAddress = new Uri(configuration["Clients:Auth"]);
        }).AddTransientHttpErrorPolicy(builder =>
            builder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(
                AuthHttpClientParameters.FirstRetryTimeSpan,
                AuthHttpClientParameters.RetryAttempts)));

        services.AddTransient<IAuthService, AuthService>();
        
        return services;
    }
}
