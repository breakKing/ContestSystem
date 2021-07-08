using ContestSystem.Providers;
using ContestSystem.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace ContestSystem.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static void AddCheckerSystemConnector(this IServiceCollection services)
        {
            services.AddSingleton<CheckerSystemService>();
        }

        public static void AddVerdicter(this IServiceCollection services)
        {
            services.AddSingleton<VerdicterService>();
        }

        public static void AddFileStorage(this IServiceCollection services)
        {
            services.AddSingleton<FileStorageService>();
        }

        public static void AddUserIdProvider(this IServiceCollection services)
        {
            services.AddSingleton<IUserIdProvider, UserIdProvider>();
        }
    }
}
