using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestSystem.Models.ExternalModels;
using ContestSystemDbStructure.Models;

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

        public async Task UpdateOnSolutionActualResultAsync(Contest contest, Solution solution)
        {
            if (solution == null || contest == null)
            {
                return;
            }

            var usersIds = new List<string>();
            var participantId = contest.ContestParticipants
                .FirstOrDefault(cp => cp.ParticipantId == solution.ParticipantId)?.ParticipantId.ToString();
            usersIds.Add(participantId);
            if (contest.ContestLocalModerators != null && contest.ContestLocalModerators.Count > 0)
            {
                var localModeratorsIds = contest.ContestLocalModerators.Select(clm => clm.LocalModeratorId.ToString())
                    .ToList();
                usersIds.AddRange(localModeratorsIds);
            }

            try
            {
                await this.Clients.Users(usersIds).SendAsync("UpdateOnSolutionActualResult",
                    SolutionActualResultExternalModel.GetFromModel(solution));
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка рассылки UpdateOnSolutionActualResult {e.Message}");
            }
        }
    }
}