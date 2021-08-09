using ContestSystem.Areas.Messenger.Services;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.FormModels;
using ContestSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public ChatsController(MainDbContext dbContext, FileStorageService fileStorage, MessengerService messenger, ILogger<ChatsController> logger, NotifierService notifier)
        {
            _dbContext = dbContext;
            _fileStorage = fileStorage;
            _messenger = messenger;
            _logger = logger;
            _notifier = notifier;
        }

        [HttpGet("{link}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> GetChat(string link, ulong? offset = null, ulong? count = null)
        {
            return null;
        }

        [HttpPost("{link}/invite/{userId}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> InviteToChat(string link, long userId)
        {
            return null;
        }

        [HttpPost("{link}/users/{userId}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> AddUserToChat(string link, long userId)
        {
            return null;
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
        public async Task<IActionResult> EditMessage(string link, long messageId, [FromBody] ChatMessageForm chatMessageForm)
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
