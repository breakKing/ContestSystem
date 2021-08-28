using ContestSystem.Hubs;
using ContestSystem.Models.ExternalModels;
using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using ContestSystemDbStructure.Models.Messenger;
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

            if (contest.ContestOrganizers != null && contest.ContestOrganizers.Count > 0)
            {
                var organizersIds = contest.ContestOrganizers.Select(co => co.OrganizerId.ToString()).ToList();
                usersIds.AddRange(organizersIds);
            }

            var actualResult = SolutionActualResultExternalModel.GetFromModel(solution);

            await SignalRSendAsync(usersIds, "UpdateOnSolutionActualResult", actualResult);
        }

        public async Task UpdateOnChatEventsAsync(ChatEvent chatEvent, List<ChatUser> chatUsers,
            ChatUserExternalModel initiator, ChatUserExternalModel affectedUser)
        {
            if (chatEvent == null || chatUsers == null || chatUsers.Count == 0)
            {
                return;
            }

            var usersIds = chatUsers.Select(cu => cu.UserId.ToString()).ToList();

            var historyEntry = ChatHistoryEntry.GetFromModel(chatEvent, affectedUser, initiator);

            await SignalRSendAsync(usersIds, "UpdateOnChatHistory", historyEntry);
        }

        public async Task UpdateOnChatMessagesAsync(ChatMessage chatMessage, List<ChatUser> chatUsers,
            ChatUserExternalModel initiator)
        {
            if (chatMessage == null || chatUsers == null || chatUsers.Count == 0)
            {
                return;
            }

            var usersIds = chatUsers.Select(cu => cu.UserId.ToString()).ToList();

            var historyEntry = ChatHistoryEntry.GetFromModel(chatMessage, initiator);

            await SignalRSendAsync(usersIds, "UpdateOnChatHistory", historyEntry);
        }

        public async Task UpdateOnUserStatsAsync(MonitorEntry monitorEntry)
        {
            if (monitorEntry == null)
            {
                return;
            }

            var userIds = new List<string>
            {
                monitorEntry.UserId.ToString()
            };

            await SignalRSendAsync(userIds, "UpdateOnUserStats", monitorEntry);
        }

        private async Task SignalRSendAsync(List<string> usersIds, string method, params object[] data)
        {
            try
            {
                if (data.Length == 1)
                {
                    await _hubContext.Clients.Users(usersIds).SendAsync(method, data[0]);
                }
                else
                {
                    await _hubContext.Clients.Users(usersIds).SendAsync(method, data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Ошибка рассылки {method}: {ex.Message}");
            }
        }
    }
}
