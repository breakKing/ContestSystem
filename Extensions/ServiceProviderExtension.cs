using ContestSystem.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Extensions
{
    public static class ServiceProviderExtension
    {
        public static void AddCheckerSystemConnector(this IServiceCollection services)
        {
            services.AddSingleton<CheckerSystemService>();
        }
        public static void AddFilesStorage(this IServiceCollection services)
        {
            services.AddSingleton<FilesStorageService>();
        }

    }
}
