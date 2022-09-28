namespace ContestSystem.GatewayApi;

public static class Startup
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        return builder;
    }

    public static WebApplication Configure(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseAuthorization();

        return app;
    }
}
