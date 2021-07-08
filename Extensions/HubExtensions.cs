using ContestSystem.Hubs;
using ContestSystem.Models.ExternalModels;
using ContestSystemDbStructure.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Extensions
{
    public static class HubExtensions
    {
        public static async Task UpdateOnSolutionActualResultAsync(this IHubContext<RealTimeHub> hubContext, Contest contest, Solution solution)
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
                await hubContext.Clients.Users(usersIds).SendAsync("UpdateOnSolutionActualResult", SolutionActualResultExternalModel.GetFromModel(solution));
            }
            catch { }
        }
    }
}
