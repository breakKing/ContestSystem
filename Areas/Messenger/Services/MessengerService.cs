using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Services;
using ContestSystemDbStructure.Models.Messenger;
using ContestSystemDbStructure.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<bool> ChatExistsAsync(MainDbContext dbContext, string link, bool isSystemChat = false)
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

        public async Task<ChatExternalModel> GetChatHistoryAsync(MainDbContext dbContext, string link, int? offset, int? count, bool isSystemChat = false)
        {
            var result = new ChatExternalModel();

            if (!await ChatExistsAsync(dbContext, link, isSystemChat))
            {
                result = null;
            }
            else
            {
                offset ??= Constants.ChatDefaultOffset;
                count ??= Constants.ChatDefaultCount;

                var chat = await dbContext.Chats.Include(ch => ch.ChatUsers)
                                                .FirstOrDefaultAsync(ch => ch.Link == link
                                                                            && ch.IsCreatedBySystem == isSystemChat);

                var messagesExpression = dbContext.ChatsMessages.Where(cm => cm.ChatId == chat.Id)
                                                                .OrderByDescending(cm => cm.SentDateTimeUTC)
                                                                .Select(cm => ChatHistoryEntry.GetFromModel(cm, false));

                var eventsExpression = dbContext.ChatsEvents.Where(ce => ce.ChatId == chat.Id)
                                                            .OrderByDescending(ce => ce.DateTimeUTC)
                                                            .Select(ce => ChatHistoryEntry.GetFromModel(ce));

                var finalExpression = messagesExpression.Concat(eventsExpression)
                                                        .Skip(offset.Value)
                                                        .Take(count.Value);

                var entries = await finalExpression.ToListAsync();
                entries = entries.OrderBy(e => e.DateTimeUTC).ToList();

                var image = _storage.GetImageInBase64(chat.ImagePath);

                result = ChatExternalModel.GetFromModel(chat, chat.ChatUsers, entries, image);
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

        public async Task<InviteStatus> InviteUserToChatAsync(MainDbContext dbContext, long userId, string link, bool isSystemChat = false)
        {
            var status = InviteStatus.Undefined;

            if (await ChatExistsAsync(dbContext, link, isSystemChat))
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
                                    bool genSuccess = await GenerateChatEventAsync(dbContext, chat.Id, ChatEventType.Invited, userId);
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

        private ChatUser UserParametersForInvite(ChatUser chatUser)
        {
            chatUser.ConfirmedByChatAdmin = true;
            chatUser.ConfirmedByThemselves = false;
            chatUser.MutedChat = false;

            return chatUser;
        }

        private async Task<bool> GenerateChatEventAsync(MainDbContext dbContext, long chatId, ChatEventType type, long userId)
        {
            var chatEvent = new ChatEvent
            {
                UserId = userId,
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

                await _notifier.UpdateOnChatEventsAsync(chatEvent, chatUsers);
            }

            return status;
        }
    }
}
