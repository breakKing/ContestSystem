using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ContestSystemDbStructure.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace ContestSystem.Models
{
    public class JwtSettingsService
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int LifeTime { get; set; }

        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.Key));
        }

        public SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(this.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256Signature);
        }

        public Claim[] GetClaimsForIdentity(User user, UserManager<User> userManager)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };
            var roles = userManager.GetRolesAsync(user).GetAwaiter().GetResult();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims.ToArray();
        }

        public SecurityTokenDescriptor GetJwtSecurityTokenDescriptor(User user, UserManager<User> userManager)
        {
            return new SecurityTokenDescriptor()
            {
                Issuer = this.Issuer,
                Audience = this.Audience,
                Subject = new ClaimsIdentity(this.GetClaimsForIdentity(user, userManager)),
                Expires = DateTime.UtcNow.AddMinutes(this.LifeTime),
                SigningCredentials = this.GetSigningCredentials()
            };
        }

        public string GenerateTokenString(User user, UserManager<User> userManager)
        {
            if (user is null)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            return handler.WriteToken(handler.CreateToken(this.GetJwtSecurityTokenDescriptor(user, userManager)));
        }
    }
}