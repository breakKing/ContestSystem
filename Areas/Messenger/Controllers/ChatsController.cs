using ContestSystem.Areas.Messenger.Services;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.FormModels;
using ContestSystem.Models.Misc;
using ContestSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContestSystem.Areas.Messenger.Controllers
{
    [Area("Messenger")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class ChatsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly FileStorageService _fileStorage;
        private readonly MessengerService _messenger;
        private readonly ILogger<ChatsController> _logger;
        private readonly NotifierService _notifier;

        private readonly string _entityName = Constants.ChatEntityName;
        private readonly Dictionary<string, string> _errorCodes = Constants.ErrorCodes[Constants.MessengerSectionName];

        public ChatsController(MainDbContext dbContext, FileStorageService fileStorage, MessengerService messenger,
            ILogger<ChatsController> logger, NotifierService notifier)
        {
            _dbContext = dbContext;
            _fileStorage = fileStorage;
            _messenger = messenger;
            _logger = logger;
            _notifier = notifier;
        }

        [HttpGet("{link}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> GetChat(string link, int? offset = null, int? count = null)
        {
            var currentUser = await HttpContext.GetCurrentUser();

            if (!await _messenger.ChatExistsAsync(_dbContext, link))
            {
                return NotFound(_errorCodes[Constants.ChatDoenstExistErrorName]);
            }

            if (!await _messenger.IsUserInChatAsync(_dbContext, currentUser.Id, link))
            {
                return BadRequest(_errorCodes[Constants.UserNotInChatErrorName]);
            }

            return Json(await _messenger.GetChatHistoryAsync(_dbContext, link, offset, count));
        }

        [HttpPost("{link}/invite/{userId}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> InviteToChat(string link, long userId)
        {
            var response = new ResponseObject<bool>();

            var currentUser = await HttpContext.GetCurrentUser();

            if (!await _messenger.ChatExistsAsync(_dbContext, link))
            {
                _logger.LogNonExistentEntityInForm("ChatUser", _entityName, currentUser.Id);
                response = ResponseObject<bool>.Fail(_errorCodes[Constants.ChatDoenstExistErrorName]);
            }
            else
            {
                if (!await _dbContext.Users.AnyAsync(u => u.Id == userId))
                {
                    _logger.LogWarning(
                        $"Главный локальный модератор чата \"{link}\" (пользователь с идентификатором {currentUser.Id}) " +
                        $"попытался пригласить несуществующего пользователя с идентификатором {userId}");
                    response = ResponseObject<bool>.Fail(
                        Constants.ErrorCodes[Constants.UserEntityName][Constants.EntityDoesntExistErrorName]);
                }
                else
                {
                    if (!await _messenger.IsUserChatAdminAsync(_dbContext, currentUser.Id, link))
                    {
                        _logger.LogWarning(
                            $"Пользователь с идентификатором {currentUser.Id} попытался пригласить пользователя с идентификатором {userId} в чат " +
                            $"\"{link}\", однако он не является главным локальным модератором чата");
                        response = ResponseObject<bool>.Fail(
                            Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                    }
                    else
                    {
                        var status = await _messenger.InviteUserToChatAsync(_dbContext, userId, link);
                        _logger.LogInviteStatus(status, _entityName, currentUser.Id, userId, link);
                        response = ResponseObject<bool>.FormResponseObjectForInvitation(status);
                    }
                }
            }

            // В случае успешного инвайта возвращается status = true и:
            // data = true, если пользователь сразу добавлен в чат
            // data = false, если админу надо дождаться подтверждение приглашённого пользователя
            return Json(response);
        }

        [HttpPost("{link}/join")]
        [AuthorizeByJwt]
        public async Task<IActionResult> AddUserToChat(string link)
        {
            var response = new ResponseObject<bool>();

            var currentUser = await HttpContext.GetCurrentUser();

            if (!await _messenger.ChatExistsAsync(_dbContext, link))
            {
                _logger.LogNonExistentEntityInForm("ChatUser", _entityName, currentUser.Id);
                response = ResponseObject<bool>.Fail(_errorCodes[Constants.ChatDoenstExistErrorName]);
            }
            else
            {
                if (await _messenger.IsUserInChatAsync(_dbContext, currentUser.Id, link))
                {
                    _logger.LogWarning($"Пользователь с идентификатором {currentUser.Id} попытался войти в чат " +
                                       $"\"{link}\", когда он уже в нём есть");
                    response = ResponseObject<bool>.Fail(
                        Constants.ErrorCodes[Constants.UserEntityName][Constants.EntityAlreadyExistsErrorName]);
                }
                else
                {
                    if (!await _messenger.IsChatJoinableAsync(_dbContext, link) &&
                        !await _messenger.IsUserInvitedInChatAsync(_dbContext, currentUser.Id, link))
                    {
                        _logger.LogWarning($"Пользователь с идентификатором {currentUser.Id} попытался войти в чат " +
                                           $"\"{link}\", однако в чат можно войти только по приглашению");
                        response = ResponseObject<bool>.Fail(
                            Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                    }
                    else
                    {
                        if (await _messenger.IsUserKickedFromChatAsync(_dbContext, currentUser.Id, link))
                        {
                            _logger.LogWarning(
                                $"Пользователь с идентификатором {currentUser.Id} попытался войти в чат " +
                                $"\"{link}\", однако ранее он был исключён главным локальным модератором чата");
                            response = ResponseObject<bool>.Fail(_errorCodes[Constants.UserKickedFromChatErrorName]);
                        }
                        else
                        {
                            bool additionSuccess =
                                await _messenger.AddUserToChatAsync(_dbContext, currentUser.Id, link);
                            if (!additionSuccess)
                            {
                                _logger.LogDbSaveError(_entityName, link);
                                response = ResponseObject<bool>.Fail(
                                    Constants.ErrorCodes[Constants.CommonSectionName][Constants.DbSaveErrorName]);
                            }
                            else
                            {
                                _logger.LogInformation(
                                    $"Пользователь с идентификатором {currentUser.Id} добавлен в чат \"{link}\"");
                                response = ResponseObject<bool>.Success(additionSuccess);
                            }
                        }
                    }
                }
            }

            return Json(response);
        }

        [HttpPost("")]
        [AuthorizeByJwt]
        public async Task<IActionResult> CreateChat(string link, [FromBody] ChatForm chatForm)
        {
            return null;
        }

        [HttpPost("{link}/messages")]
        [AuthorizeByJwt]
        public async Task<IActionResult> SendMessage(string link, [FromBody] ChatMessageForm chatMessageForm)
        {
            return null;
        }

        [HttpPut("{link}/users/{userId}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> EditUserInChat(string link, long userId, bool enable = true)
        {
            return null;
        }

        [HttpPut("{link}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> EditChat(string link, [FromBody] ChatForm chatForm)
        {
            return null;
        }

        [HttpPut("{link}/messages/{messageId}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> EditMessage(string link, long messageId,
            [FromBody] ChatMessageForm chatMessageForm)
        {
            return null;
        }

        [HttpDelete("{link}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> DeleteChat(string link)
        {
            return null;
        }

        [HttpDelete("{link}/users/{userId}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> RemoveUserFromChat(string link, long userId)
        {
            return null;
        }
    }
}