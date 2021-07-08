using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace ContestSystem.Providers
{
    public class UserIdProvider : IUserIdProvider
    {
        public virtual string GetUserId(HubConnectionContext connection)
        {
            if (long.TryParse(connection.User?.Claims?.SingleOrDefault(x => x.Type == "Id")?.Value, out var result))
            {
                return result.ToString();
            }

            return null;
        }
    }
}
