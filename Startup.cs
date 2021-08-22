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
using ContestSystemDbStructure.Models.Auth;
using Microsoft.AspNetCore.HttpOverrides;

namespace ContestSystem
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddLocalization(options => options.ResourcesPath = "Resources");

            var jwtService = new JwtSettingsService();
            Configuration.Bind("JwtConfiguration", jwtService);
            services.AddSingleton(jwtService);

            services.AddDbContext<MainDbContext>(
                x => x
                    .UseLazyLoadingProxies()
                    //.UseNpgsql(Configuration.GetConnectionString("PgsqlConnection"))
                    .UseSqlServer(Configuration.GetConnectionString("MssqlConnection"))
            );


            services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 5;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            }).AddEntityFrameworkStores<MainDbContext>();

            services.AddAuthentication(x =>
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
            services.AddControllersWithViews();

            services.AddCheckerSystemConnector();

            services.AddContestsManager();

            services.AddSolutionsManager();

            services.AddFileStorage();

            services.AddWorkspace();

            services.AddLocalizers();

            services.AddSignalR();

            services.AddUserIdProvider();

            services.AddNotifier();

            services.AddMessenger();

            services.AddSwaggerGen(c =>
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddFile("Logs/log_{Date}.txt");

            if (env.IsDevelopment())
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
        }
    }
}