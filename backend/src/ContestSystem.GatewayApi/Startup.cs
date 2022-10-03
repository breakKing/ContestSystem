using System.Reflection;
using ContestSystem.GatewayApi.Auth;
using ContestSystem.GatewayApi.Common.Extensions;
using ContestSystem.GatewayApi.Common.Interfaces;
using ContestSystem.GatewayApi.Common.Services;

namespace ContestSystem.GatewayApi;

public static class Startup
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddFastEndpoints();
        builder.Services.AddMappersFromAssembly(Assembly.GetExecutingAssembly());
        builder.Services.AddSingleton<IIdsHasher, IdsHasher>();

        builder.Services.AddAuthServices(builder.Configuration);
        
        return builder;
    }

    public static WebApplication Configure(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseFastEndpoints();

        return app;
    }
}
