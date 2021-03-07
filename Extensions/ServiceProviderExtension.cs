using ContestSystem.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ContestSystem.Extensions
{
    public static class ServiceProviderExtension
    {
        public static void AddCheckerSystemConnector(this IServiceCollection services)
        {
            services.AddSingleton<CheckerSystemService>();
        }
    }
}
