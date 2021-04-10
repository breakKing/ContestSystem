using ContestSystem.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Globalization;
using ContestSystem.Models;
using ContestSystem.Models.DbContexts;
using ContestSystemDbStructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

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
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            var jwtService = new JwtSettingsService();
            Configuration.Bind("JwtConfiguration", jwtService);
            services.AddSingleton<JwtSettingsService>(jwtService);

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
                });
            services.AddControllersWithViews();

            services.AddCheckerSystemConnector();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            CultureInfo[] supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("ru"),
            };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "CatchAll",
                    pattern: "{*url}",
                    defaults: new {controller = "Home", action = "Index"}
                );
            });
        }
    }
}