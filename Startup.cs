using ContestSystem.Extensions;
using ContestSystem.Models;
using ContestSystem.Models.DbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using ContestSystem.Hubs;
using System.Threading.Tasks;
using ContestSystem.DbStructure.Models.Auth;
using Microsoft.AspNetCore.HttpOverrides;

namespace ContestSystem
{
    public static class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
        {
            //services.AddLocalization(options => options.ResourcesPath = "Resources");

            var jwtService = new JwtSettingsService();
            builder.Configuration.Bind("JwtConfiguration", jwtService);
            builder.Services.AddSingleton(jwtService);

            builder.Services.AddDbContext<MainDbContext>(
                x => x
                    .UseLazyLoadingProxies()
                    .UseSqlServer(builder.Configuration.GetConnectionString("MssqlConnection"))
            );


            builder.Services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            }).AddEntityFrameworkStores<MainDbContext>();

            builder.Services.AddAuthentication(x =>
                {
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtService.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtService.Audience,
                        ValidateLifetime = true,

                        IssuerSigningKey = jwtService.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/real_time_hub"))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
            builder.Services.AddControllersWithViews();

            builder.Services.AddCheckerSystemConnector();

            builder.Services.AddContestsManager();

            builder.Services.AddSolutionsManager();

            builder.Services.AddFileStorage();

            builder.Services.AddWorkspace();

            builder.Services.AddLocalizers();

            builder.Services.AddSignalR();

            builder.Services.AddUserIdProvider();

            builder.Services.AddNotifier();

            builder.Services.AddMessenger();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v0.1", new OpenApiInfo { Title = "ContestSystem API v0.1 beta", Version = "v0.1" });
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "JWT-Auth"
                    }
                };
                c.AddSecurityDefinition("JWT-Auth", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securityScheme, new[] { "JWT-Auth" } }
                });
            });

            return builder;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static WebApplication Configure(this WebApplication app)
        {
            var loggerFactory = app.Services.GetService<ILoggerFactory>();
            loggerFactory.AddFile("Logs/log_{Date}.txt");

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v0.1/swagger.json", "ContestSystem API v0.1 beta");
                });
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseWebSockets();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<RealTimeHub>("/api/real_time_hub");
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "CatchAll",
                    pattern: "{*url}",
                    defaults: new { controller = "Home", action = "Index" }
                );
            });

            return app;
        }
    }
}