using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Services;
using ContestSystemDbStructure.Models.Messenger;
using ContestSystemDbStructure.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ContestSystem.Models.FormModels;
using ContestSystem.Models.Misc;
using System.Collections.Generic;
using ContestSystemDbStructure.Models;

namespace ContestSystem.Areas.Messenger.Services
{
    public class MessengerService
    {
        private readonly NotifierService _notifier;
        private readonly FileStorageService _storage;

        public MessengerService(NotifierService notifier, FileStorageService storage)
        {
            _notifier = notifier;
            _storage = storage;
        }

        public async Task<bool> ChatExistsAsync(MainDbContext dbContext, string link)
        {
            return await dbContext.Chats.AnyAsync(ch => ch.Link == link);
        }

        public async Task<bool> ChatExistsAsync(MainDbContext dbContext, string link, bool isSystemChat)
        {
            return await dbContext.Chats.AnyAsync(ch => ch.Link == link
                                                        && ch.IsCreatedBySystem == isSystemChat);
        }

        public async Task<bool> IsUserInChatAsync(MainDbContext dbContext, long userId, string link)
        {
            return await dbContext.ChatsUsers.AnyAsync(cu => cu.UserId == userId
                                                             && cu.Chat.Link == link
                                                             && cu.ConfirmedByChatAdmin
                                                             && cu.ConfirmedByThemselves);
        }

        public async Task<ChatExternalModel> GetChatHistoryAsync(MainDbContext dbContext, string link, int? offset,
            int? count)
        {
            var result = new ChatExternalModel();

            if (!await ChatExistsAsync(dbContext, link))
            {
                result = null;
            }
            else
            {
                offset ??= Constants.ChatDefaultOffset;
                count ??= Constants.ChatDefaultCount;

                var chat = await dbContext.Chats.Include(ch => ch.ChatUsers)
                    .FirstOrDefaultAsync(ch => ch.Link == link);

                var extenalChatUsers =
                    chat.ChatUsers.ConvertAll(cu => GetChatUserAsync(dbContext, cu).GetAwaiter().GetResult());

                var messagesExpression = dbContext.ChatsMessages.Where(cm => cm.ChatId == chat.Id)
                    .OrderByDescending(cm => cm.SentDateTimeUTC)
                    .AsEnumerable()
                    .Select(cm =>
                        ChatHistoryEntry.GetFromModel(cm,
                            extenalChatUsers.FirstOrDefault(ecu => ecu.Id == cm.SenderId)));

                var eventsExpression = dbContext.ChatsEvents.Where(ce => ce.ChatId == chat.Id)
                    .OrderByDescending(ce => ce.DateTimeUTC)
                    .AsEnumerable()
                    .Select(ce => ChatHistoryEntry.GetFromModel(ce,
                        extenalChatUsers.FirstOrDefault(ecu => ecu.Id == ce.AffectedUserId),
                        extenalChatUsers.FirstOrDefault(ecu => ecu.Id == ce.InitiatorId)));

                var finalExpression = messagesExpression.Concat(eventsExpression)
                    .Skip(offset.Value)
                    .Take(count.Value);

                var entries = finalExpression.ToList();
                entries = entries.OrderBy(e => e.DateTimeUTC).ToList();

                var image = _storage.GetImageInBase64(chat.ImagePath);

                result = ChatExternalModel.GetFromModel(chat,
                    chat.ChatUsers.ConvertAll(cu => GetChatUserAsync(dbContext, cu).GetAwaiter().GetResult()), entries,
                    image);
            }

            return result;
        }

        public async Task<bool> IsUserChatAdminAsync(MainDbContext dbContext, long userId, string link)
        {
            return await dbContext.Chats.AnyAsync(ch => ch.Link == link
                                                        && ch.AdminId == userId);
        }

        public async Task<bool> IsChatJoinableAsync(MainDbContext dbContext, string link)
        {
            var chat = await dbContext.Chats.FirstOrDefaultAsync(ch => ch.Link == link);

            if (chat == null)
            {
                return false;
            }

            return chat.AnyoneCanJoin;
        }

        public async Task<InviteStatus> InviteUserToChatAsync(MainDbContext dbContext, long userId, string link)
        {
            var status = InviteStatus.Undefined;

            if (await ChatExistsAsync(dbContext, link, false))
            {
                if (await IsUserInChatAsync(dbContext, userId, link))
                {
                    status = InviteStatus.UserAlreadyInEntity;
                }
                else
                {
                    if (await IsUserInvitedInChatAsync(dbContext, userId, link))
                    {
                        status = InviteStatus.UserAlreadyInvited;
                    }
                    else
                    {
                        var chat = await GetChatByLinkAsync(dbContext, link);

                        if (chat == null)
                        {
                            status = InviteStatus.DbSaveError;
                        }
                        else
                        {
                            if (await UserRequestedJoinToChatAsync(dbContext, userId, link) || chat.AnyoneCanJoin)
                            {
                                bool addingSuccess = await AddUserToChatAsync(dbContext, userId, link);
                                if (!addingSuccess)
                                {
                                    status = InviteStatus.DbSaveError;
                                }
                                else
                                {
                                    status = InviteStatus.Added;
                                }
                            }
                            else
                            {
                                var chatUser = new ChatUser();

                                if (await IsUserKickedFromChatAsync(dbContext, userId, link))
                                {
                                    chatUser = await GetChatUserAsync(dbContext, userId, link);

                                    chatUser = UserParametersForInvite(chatUser);

                                    dbContext.ChatsUsers.Update(chatUser);
                                }
                                else
                                {
                                    chatUser = new ChatUser
                                    {
                                        ChatId = chat.Id,
                                        UserId = userId,
                                    };

                                    chatUser = UserParametersForInvite(chatUser);

                                    await dbContext.ChatsUsers.AddAsync(chatUser);
                                }

                                bool saveSuccess = await dbContext.SecureSaveAsync();
                                if (!saveSuccess)
                                {
                                    status = InviteStatus.DbSaveError;
                                }
                                else
                                {
                                    bool genSuccess = await GenerateChatEventAsync(dbContext, chat.Id,
                                        ChatEventType.Invited, userId);
                                    if (!genSuccess)
                                    {
                                        status = InviteStatus.DbSaveError;
                                    }
                                    else
                                    {
                                        status = InviteStatus.Pending;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return status;
        }

        public async Task<bool> AddUserToChatAsync(MainDbContext dbContext, long userId, string link)
        {
            var status = false;

            var chat = await dbContext.Chats.FirstOrDefaultAsync(ch => ch.Link == link);

            var chatUser = await dbContext.ChatsUsers.FirstOrDefaultAsync(cu => cu.UserId == userId
                && cu.Chat.Link == link);

            if (chat != null)
            {
                if (chatUser == null)
                {
                    chatUser = new ChatUser
                    {
                        ChatId = chat.Id,
                        UserId = userId,
                        MutedChat = false,
                        ConfirmedByChatAdmin = true,
                        ConfirmedByThemselves = true
                    };

                    await dbContext.ChatsUsers.AddAsync(chatUser);
                }
                else
                {
                    chatUser.ConfirmedByChatAdmin = true;
                    chatUser.ConfirmedByThemselves = true;

                    dbContext.ChatsUsers.Update(chatUser);
                }

                status = await dbContext.SecureSaveAsync();
            }

            if (status)
            {
                await GenerateChatEventAsync(dbContext, chat.Id, ChatEventType.Joined, userId);
            }

            return status;
        }

        public async Task<FormCheckStatus> CheckChatFormAsync(MainDbContext dbContext, ChatForm form)
        {
            var status = FormCheckStatus.Undefined;

            bool allUserExist = true;

            foreach (var userId in form.InitialUsers)
            {
                if (!await dbContext.Users.AnyAsync(u => u.Id == userId))
                {
                    allUserExist = false;
                    break;
                }
            }

            if (!allUserExist)
            {
                status = FormCheckStatus.NonExistentUser;
            }
            else
            {
                status = FormCheckStatus.Correct;
            }

            return status;
        }

        public async Task<CreationStatusData<string>> CreateChatAsync(MainDbContext dbContext, ChatForm form,
            ChatType type = ChatType.Custom, long? contestId = null, long? participantId = null)
        {
            var statusData = new CreationStatusData<string>
            {
                Status = CreationStatus.Undefined,
                Id = null
            };

            form.InitialUsers ??= new List<long>();

            if (form.InitialUsers.Count > 0 && !form.InitialUsers.Contains(form.AdminId))
            {
                form.InitialUsers.Add(form.AdminId);
            }

            var chat = new Chat
            {
                Name = form.Name,
                AnyoneCanJoin = form.AnyoneCanJoin,
                AdminId = form.AdminId,
                Type = type,
                IsCreatedBySystem = (type != ChatType.Custom)
            };

            if (type == ChatType.ContestAnnouncements ||
                type == ChatType.ContestParticipant ||
                type == ChatType.ContestModerator)
            {
                chat.ContestId = contestId;
            }

            await dbContext.Chats.AddAsync(chat);

            bool saveSuccess = await dbContext.SecureSaveAsync();
            if (!saveSuccess)
            {
                statusData.Status = CreationStatus.DbSaveError;
            }
            else
            {
                chat.ImagePath = await _storage.SaveChatImageAsync(chat.Id, form.Image);
                if (type == ChatType.ContestAnnouncements ||
                    type == ChatType.ContestModerator)
                {
                    chat.Link = GenerateContestChatLink(chat, chat.Contest);
                }
                else if (type == ChatType.ContestParticipant)
                {
                    var participant = chat.Contest.ContestParticipants.FirstOrDefault(cp =>
                        cp.ParticipantId == participantId.GetValueOrDefault(-1));
                    chat.Link = GenerateContestChatLink(chat, chat.Contest, participant);
                }
                else
                {
                    chat.Link = GenerateCustomChatLink(chat);
                }

                form.InitialUsers.ForEach(u => chat.ChatUsers.Add(new ChatUser
                {
                    ChatId = chat.Id,
                    UserId = u,
                    ConfirmedByChatAdmin = true,
                    ConfirmedByThemselves = chat.IsCreatedBySystem,
                    MutedChat = false
                }));

                dbContext.Chats.Update(chat);

                saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    statusData.Status = CreationStatus.DbSaveError;
                    await DeleteChatAsync(dbContext, chat);
                }
                else
                {
                    statusData.Status = CreationStatus.Success;
                    statusData.Id = chat.Link;
                }
            }

            return statusData;
        }

        public async Task<FormCheckStatus> CheckChatMessageFormAsync(MainDbContext dbContext, ChatMessageForm form)
        {
            var status = FormCheckStatus.Correct;

            bool userExist = await dbContext.Users.AnyAsync(u => u.Id == form.UserId);

            if (!userExist)
            {
                status = FormCheckStatus.NonExistentUser;
            }
            else
            {
                bool isUserInChat = await IsUserInChatAsync(dbContext, form.UserId, form.ChatLink);

                if (!isUserInChat)
                {
                    status = FormCheckStatus.NonExistentChatUser;
                }
                else
                {
                    status = FormCheckStatus.Correct;
                }
            }

            return status;
        }

        public async Task<CreationStatusData<long?>> SendChatMessageAsync(MainDbContext dbContext, ChatMessageForm form)
        {
            var statusData = new CreationStatusData<long?>
            {
                Status = CreationStatus.Undefined,
                Id = null
            };

            var chat = await dbContext.Chats.FirstOrDefaultAsync(c => c.Link == form.ChatLink);

            if (chat != null)
            {
                var now = DateTime.UtcNow;

                var chatMessage = new ChatMessage
                {
                    ChatId = chat.Id,
                    SenderId = form.UserId,
                    Text = form.Text,
                    SentDateTimeUTC = now,
                    UpdateDateTimeUTC = now
                };

                await dbContext.ChatsMessages.AddAsync(chatMessage);

                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    statusData.Status = CreationStatus.DbSaveError;
                }
                else
                {
                    statusData.Status = CreationStatus.Success;
                    statusData.Id = chatMessage.Id;

                    var chatUsers = await dbContext.ChatsUsers.Where(cu => cu.ChatId == chat.Id)
                        .ToListAsync();

                    var sender = await GetChatUserAsync(dbContext,
                        chatUsers.FirstOrDefault(cu => cu.UserId == chatMessage.SenderId));
                    await _notifier.UpdateOnChatMessagesAsync(chatMessage, chatUsers, sender);
                }
            }
            else
            {
                statusData.Status = CreationStatus.Undefined;
            }

            return statusData;
        }

        public async Task<DeletionStatus> RemoveUserFromChatAsync(MainDbContext dbContext, Chat chat, long userId)
        {
            var status = DeletionStatus.Undefined;
            if (chat == null)
            {
                status = DeletionStatus.NotExistentEntity;
            }
            else
            {
                var chatUser = chat.ChatUsers.FirstOrDefault(cu => cu.UserId == userId);
                if (chatUser == null)
                {
                    status = DeletionStatus.NotExistentEntity;
                }
                else
                {
                    dbContext.ChatsUsers.Remove(chatUser);
                    bool saveSuccess = await dbContext.SecureSaveAsync();

                    if (!saveSuccess)
                    {
                        status = DeletionStatus.DbSaveError;
                    }
                    else
                    {
                        status = DeletionStatus.Success;
                    }
                }
            }

            return status;
        }

        public async Task<DeletionStatus> DeleteChatAsync(MainDbContext dbContext, Chat chat)
        {
            var status = DeletionStatus.Undefined;
            if (chat == null)
            {
                status = DeletionStatus.NotExistentEntity;
            }
            else
            {
                _storage.DeleteFileAsync(chat.ImagePath);
                dbContext.Chats.Remove(chat);
                bool saveSuccess = await dbContext.SecureSaveAsync();
                if (!saveSuccess)
                {
                    status = DeletionStatus.DbSaveError;
                }
                else
                {
                    status = DeletionStatus.Success;
                }
            }

            return status;
        }

        public async Task<bool> IsUserInvitedInChatAsync(MainDbContext dbContext, long userId, string link)
        {
            return await dbContext.ChatsUsers.AnyAsync(cu => cu.UserId == userId
                                                             && cu.Chat.Link == link
                                                             && cu.ConfirmedByChatAdmin
                                                             && !cu.ConfirmedByThemselves);
        }

        public async Task<bool> IsUserKickedFromChatAsync(MainDbContext dbContext, long userId, string link)
        {
            return await dbContext.ChatsUsers.AnyAsync(cu => cu.UserId == userId
                                                             && cu.Chat.Link == link
                                                             && !cu.ConfirmedByChatAdmin
                                                             && !cu.ConfirmedByThemselves);
        }

        private async Task<bool> UserRequestedJoinToChatAsync(MainDbContext dbContext, long userId, string link)
        {
            return await dbContext.ChatsUsers.AnyAsync(cu => cu.UserId == userId
                                                             && cu.Chat.Link == link
                                                             && !cu.ConfirmedByChatAdmin
                                                             && cu.ConfirmedByThemselves);
        }

        private async Task<Chat> GetChatByLinkAsync(MainDbContext dbContext, string link)
        {
            return await dbContext.Chats.FirstOrDefaultAsync(ch => ch.Link == link);
        }

        private async Task<ChatUser> GetChatUserAsync(MainDbContext dbContext, long userId, string link)
        {
            return await dbContext.ChatsUsers.FirstOrDefaultAsync(cu => cu.UserId == userId
                                                                        && cu.Chat.Link == link);
        }

        private async Task<ChatUserExternalModel> GetChatUserAsync(MainDbContext dbContext, ChatUser chatUser)
        {
            var user = new ChatUserExternalModel();

            if (chatUser != null)
            {
                user = new ChatUserExternalModel
                {
                    ChatId = chatUser.ChatId,
                    ChatLink = chatUser.Chat.Link,
                    Id = chatUser.UserId,
                    Name = string.Empty
                };

                string name = string.Empty;

                if (chatUser.Chat.Type == ChatType.ContestAnnouncements ||
                    chatUser.Chat.Type == ChatType.ContestParticipant)
                {
                    var contestId = chatUser.Chat.ContestId.GetValueOrDefault(-1);

                    if (await dbContext.ContestsOrganizers.AnyAsync(co =>
                        co.ContestId == contestId && co.OrganizerId == chatUser.UserId))
                    {
                        name = (await dbContext.ContestsOrganizers.FirstOrDefaultAsync(co =>
                                co.ContestId == contestId
                                && co.OrganizerId == chatUser.UserId))
                            .Alias;
                    }
                    else if (await dbContext.ContestsParticipants.AnyAsync(cp =>
                        cp.ContestId == contestId && cp.ParticipantId == chatUser.UserId && cp.ConfirmedByParticipant &&
                        cp.ConfirmedByOrganizer))
                    {
                        name = (await dbContext.ContestsParticipants.FirstOrDefaultAsync(cp => cp.ContestId == contestId
                                && cp.ParticipantId == chatUser.UserId))
                            .Alias;
                    }
                    else
                    {
                        name = chatUser.User.FullName;
                    }
                }
                else
                {
                    name = chatUser.User.FullName;
                }

                user.Name = name;
            }

            return user;
        }

        private ChatUser UserParametersForInvite(ChatUser chatUser)
        {
            chatUser.ConfirmedByChatAdmin = true;
            chatUser.ConfirmedByThemselves = false;
            chatUser.MutedChat = false;

            return chatUser;
        }

        private async Task<bool> GenerateChatEventAsync(MainDbContext dbContext, long chatId, ChatEventType type,
            long? userId)
        {
            var chat = await dbContext.Chats.FirstOrDefaultAsync(c => c.Id == chatId);

            var chatEvent = new ChatEvent
            {
                AffectedUserId = userId,
                InitiatorId = chat?.AdminId ?? null,
                ChatId = chatId,
                DateTimeUTC = DateTime.UtcNow,
                Type = type
            };

            await dbContext.ChatsEvents.AddAsync(chatEvent);

            bool status = await dbContext.SecureSaveAsync();

            if (status)
            {
                var chatUsers = await dbContext.ChatsUsers.Where(cu => cu.ChatId == chatId)
                    .ToListAsync();

                var initiator = chatEvent.InitiatorId == null
                    ? await GetChatUserAsync(dbContext,
                        chatUsers.FirstOrDefault(cu => cu.UserId == chatEvent.InitiatorId))
                    : null;
                var affectedUser = chatEvent.AffectedUserId == null
                    ? await GetChatUserAsync(dbContext,
                        chatUsers.FirstOrDefault(cu => cu.UserId == chatEvent.AffectedUserId))
                    : null;
                await _notifier.UpdateOnChatEventsAsync(chatEvent, chatUsers, initiator, affectedUser);
            }

            return status;
        }

        private string GenerateCustomChatLink(Chat chat)
        {
            var link = "";

            if (chat != null && chat.Type == ChatType.Custom)
            {
                link = $"users-{chat.Id}";
            }

            return link;
        }

        private string GenerateContestChatLink(Chat chat, Contest contest, ContestParticipant participant = null)
        {
            var link = "";

            if (chat != null && contest != null)
            {
                switch (chat.Type)
                {
                    case ChatType.ContestParticipant:
                        if (participant != null && participant.ContestId == contest.Id)
                        {
                            link = $"contest-participant-{contest.Id}-{participant.ParticipantId}";
                        }

                        break;
                    case ChatType.ContestAnnouncements:
                        link = $"contest-announcements-{contest.Id}";
                        break;
                    case ChatType.ContestModerator:
                        link = $"contest-moderator-{contest.Id}";
                        break;
                    default:
                        break;
                }
            }

            return link;
        }
    }
}