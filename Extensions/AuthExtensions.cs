using System.Linq;
using System.Threading.Tasks;
using ContestSystemDbStructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ContestSystem.Extensions
{
    public static class AuthExtensions
    {
        public static long GetUserId(this HttpContext httpContext)
        {
            if (long.TryParse(httpContext.User?.Claims?.SingleOrDefault(x => x.Type == "Id")?.Value, out var result))
            {
                return result;
            }

            return default(long);
        }

        public static async Task<User> GetCurrentUser(this HttpContext httpContext,
            UserManager<User> userManager = null)
        {
            userManager ??= httpContext.RequestServices.GetRequiredService<UserManager<User>>();
            return await userManager.FindByIdAsync(httpContext.GetUserId().ToString());
        }
    }
}