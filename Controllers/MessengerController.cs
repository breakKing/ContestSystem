using ContestSystem.Hubs;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.FormModels;
using ContestSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessengerController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly FileStorageService _fileStorage;
        private readonly MessengerService _messenger;
        private readonly ILogger<MessengerController> _logger;
        private readonly ILogger<RealTimeHub> _loggerForHub;
        private readonly IHubContext<RealTimeHub> _hubContext;

        public MessengerController(MainDbContext dbContext, FileStorageService fileStorage, MessengerService messenger, ILogger<MessengerController> logger, ILogger<RealTimeHub> loggerForHub, IHubContext<RealTimeHub> hubContext)
        {
            _dbContext = dbContext;
            _fileStorage = fileStorage;
            _messenger = messenger;
            _logger = logger;
            _loggerForHub = loggerForHub;
            _hubContext = hubContext;
        }

        [HttpGet("chat/{link}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> GetChat(string link, ulong? offset = null, ulong? count = null)
        {
            return null;
        }

        [HttpPost("chat/{link}/invite/{userId}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> InviteToChat(string link, long userId)
        {
            return null;
        }

        [HttpPost("chat/{link}/add-user/{userId}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> AddUserToChat(string link, long userId)
        {
            return null;
        }

        [HttpPost("chat/{link}/remove-user/{userId}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> RemoveUserFromChat(string link, long userId)
        {
            return null;
        }

        [HttpPost("chat/{link}/send-message")]
        [AuthorizeByJwt]
        public async Task<IActionResult> SendMessage(string link, ChatMessageForm chatMessageForm)
        {
            return null;
        }

        [HttpPost("chat/{link}/mute-for-user/{userId}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> MuteChat(string link, long userId)
        {
            return null;
        }

        [HttpPost("chat/{link}/kick/{userId}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> KickFromChat(string link, long userId)
        {
            return null;
        }

        [HttpPost("chat/{link}/create")]
        [AuthorizeByJwt]
        public async Task<IActionResult> CreateChat(string link, ChatForm chatForm)
        {
            return null;
        }

        [HttpPut("chat/{link}/edit")]
        [AuthorizeByJwt]
        public async Task<IActionResult> EditChat(string link, ChatForm chatForm)
        {
            return null;
        }

        [HttpDelete("chat/{link}/delete")]
        [AuthorizeByJwt]
        public async Task<IActionResult> DeleteChat(string link)
        {
            return null;
        }
    }
}
