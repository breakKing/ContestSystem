using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace ContestSystem.Models.Attributes
{
    public class AuthorizeByJwtAttribute : AuthorizeAttribute
    {
        public AuthorizeByJwtAttribute() : base()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }

        public AuthorizeByJwtAttribute(string policy) : base(policy)
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }
}