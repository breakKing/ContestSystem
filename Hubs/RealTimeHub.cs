using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContestSystem.Hubs
{
    [AuthorizeByJwt]
    public class RealTimeHub : Hub
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<RealTimeHub> _logger;

        public RealTimeHub(MainDbContext dbContext, ILogger<RealTimeHub> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}