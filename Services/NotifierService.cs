using ContestSystem.Hubs;
using ContestSystem.Models.ExternalModels;
using ContestSystemDbStructure.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Services
{
    public class NotifierService
    {
        private readonly IHubContext<RealTimeHub> _hubContext;
        private readonly ILogger<NotifierService> _logger;

        public NotifierService(IHubContext<RealTimeHub> hubContext, ILogger<NotifierService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task UpdateOnSolutionActualResultAsync(Contest contest, Solution solution)
        {
            if (solution == null || contest == null)
            {
                return;
            }

            var usersIds = new List<string>();
            var participantId = contest.ContestParticipants.FirstOrDefault(cp => cp.ParticipantId == solution.ParticipantId)?.ParticipantId.ToString();
            usersIds.Add(participantId);

            if (contest.ContestLocalModerators != null && contest.ContestLocalModerators.Count > 0)
            {
                var localModeratorsIds = contest.ContestLocalModerators.Select(clm => clm.LocalModeratorId.ToString()).ToList();
                usersIds.AddRange(localModeratorsIds);
            }

            try
            {
                await _hubContext.Clients.Users(usersIds).SendAsync("UpdateOnSolutionActualResult", SolutionActualResultExternalModel.GetFromModel(solution));
            }
            catch (Exception e)
            {
                _logger.LogError($"Ошибка рассылки UpdateOnSolutionActualResult: {e.Message}");
            }
        }
    }
}
