using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystemDbStructure.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        public static void SetRefreshTokenCookie(this HttpContext httpContext, string refreshToken, int maxAge)
        {
            var options = new CookieOptions
            {
                MaxAge = TimeSpan.FromHours(maxAge),
                Path = "/api/auth",
                HttpOnly = true
            };

            httpContext.Response.Cookies.Append(Constants.RefreshTokenCookieName, refreshToken, options);
        }

        public static async Task<List<Session>> GetUserSessionsAsync(this UserManager<User> userManager, MainDbContext dbContext, long userId)
        {
            if (await userManager.FindByIdAsync(userId.ToString()) == null)
            {
                return new List<Session>();
            }

            var sessions = await dbContext.Sessions.Where(s => s.UserId == userId)
                                            .OrderBy(s => s.StartTimeUTC)
                                            .ToListAsync();

            bool needToReload = false;
            for (int i = 0; i < sessions.Count; i++)
            {
                Session session = sessions[i];
                if (session.StartTimeUTC.AddHours(session.ExpiresInHours) <= DateTime.UtcNow)
                {
                    needToReload = true; 
                    dbContext.Sessions.Remove(session);
                }
            }

            if (needToReload)
            {
                await dbContext.SecureSaveAsync();

                sessions = await dbContext.Sessions.Where(s => s.UserId == userId)
                                                    .OrderBy(s => s.StartTimeUTC)
                                                    .ToListAsync();
            }

            return sessions;
        }

        public static async Task<Session> GetUserSessionByRefreshTokenAsync(this UserManager<User> userManager, MainDbContext dbContext, long userId,
            string refreshToken)
        {
            if (await userManager.FindByIdAsync(userId.ToString()) == null)
            {
                return null;
            }

            var now = DateTime.UtcNow;

            return await dbContext.Sessions.FirstOrDefaultAsync(s => s.UserId == userId && s.RefreshToken.ToString() == refreshToken
                                                                        && s.StartTimeUTC.AddHours(s.ExpiresInHours) > now);
        }

        public static async Task<Session> GetSessionByRefreshTokenAndFingerprintAsync(this UserManager<User> userManager, MainDbContext dbContext,
            string refreshToken, string fingerprint)
        {
            var now = DateTime.UtcNow;

            return await dbContext.Sessions.FirstOrDefaultAsync(s => s.Fingerprint == fingerprint && s.RefreshToken.ToString() == refreshToken
                                                                        && s.StartTimeUTC.AddHours(s.ExpiresInHours) > now);
        }

        public static async Task<Session> GetSessionByUserAndFingerprintAsync(this UserManager<User> userManager, MainDbContext dbContext,
            long userId, string fingerprint)
        {
            if (await userManager.FindByIdAsync(userId.ToString()) == null)
            {
                return null;
            }

            var now = DateTime.UtcNow;

            return await dbContext.Sessions.FirstOrDefaultAsync(s => s.Fingerprint == fingerprint && s.UserId == userId
                                                                        && s.StartTimeUTC.AddHours(s.ExpiresInHours) > now);
        }

        public static async Task<string> CreateUserSessionAsync(this UserManager<User> userManager, MainDbContext dbContext, long userId, 
            int durationInHours, string fingerprint)
        {
            if (await userManager.FindByIdAsync(userId.ToString()) == null)
            {
                return null;
            }

            var session = new Session
            {
                UserId = userId,
                RefreshToken = GenereteRefreshToken(),
                Fingerprint = fingerprint,
                StartTimeUTC = DateTime.UtcNow,
                ExpiresInHours = durationInHours
            };

            await dbContext.Sessions.AddAsync(session);

            if (await dbContext.SecureSaveAsync())
            {
                return session.RefreshToken.ToString();
            }

            return null;
        }

        public static async Task<string> UpdateRefreshTokenAsync(this UserManager<User> userManager, MainDbContext dbContext, long userId,
            string oldRefreshToken)
        {
            if (await userManager.FindByIdAsync(userId.ToString()) == null)
            {
                return null;
            }

            var session = await userManager.GetUserSessionByRefreshTokenAsync(dbContext, userId, oldRefreshToken);

            if (session == null)
            {
                return null;
            }

            session.RefreshToken = GenereteRefreshToken();
            session.StartTimeUTC = DateTime.UtcNow;

            dbContext.Sessions.Update(session);

            if (await dbContext.SecureSaveAsync())
            {
                return session.RefreshToken.ToString();
            }

            return null;
        }

        public static async Task<bool> RemoveUserSessionAsync(this UserManager<User> userManager, MainDbContext dbContext, long userId,
            string refreshToken)
        {
            if (await userManager.FindByIdAsync(userId.ToString()) == null)
            {
                return false;
            }

            var session = await userManager.GetUserSessionByRefreshTokenAsync(dbContext, userId, refreshToken);

            if (session == null)
            {
                return false;
            }

            dbContext.Sessions.Remove(session);

            return await dbContext.SecureSaveAsync();
        }

        public static async Task<bool> DropAllUserSessionsAsync(this UserManager<User> userManager, MainDbContext dbContext, long userId)
        {
            if (await userManager.FindByIdAsync(userId.ToString()) == null)
            {
                return false;
            }

            var sessions = await dbContext.Sessions.Where(s => s.UserId == userId).ToListAsync();

            if (sessions != null && sessions.Count > 0)
            {
                dbContext.Sessions.RemoveRange(sessions);

                return await dbContext.SecureSaveAsync();
            }

            return true;
        }

        private static Guid GenereteRefreshToken()
        {
            return Guid.NewGuid();
        }
    }
}