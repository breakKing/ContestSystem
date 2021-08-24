using ContestSystem.Areas.Contests.Services;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using ContestSystem.Models.Misc;
using ContestSystemDbStructure.Models;
using ContestSystemDbStructure.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Areas.Contests.Controllers
{
    [Area("Contests")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class ParticipantsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<ParticipantsController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly ContestsManagerService _contestsManager;

        private readonly string _entityName = Constants.ContestEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public ParticipantsController(MainDbContext dbContext, ILogger<ParticipantsController> logger, UserManager<User> userManager,
            ContestsManagerService contestsManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _contestsManager = contestsManager;

            _errorCodes = Constants.ErrorCodes[_entityName];
        }

        [HttpGet("{contestId}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetParticipants(long contestId)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            var contestParticipants =
                await _dbContext.ContestsParticipants.Where(cp => cp.ContestId == contestId).ToListAsync();

            return Json(contestParticipants.ConvertAll(ParticipantExternalModel.GetFromModel));
        }

        [HttpGet("{contestId}/stats/{userId}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetParticipantStats(long contestId, long userId)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }

            var currentUser = await HttpContext.GetCurrentUser(_userManager);

            if (currentUser.Id != userId
                && !await _contestsManager.IsUserContestLocalModeratorAsync(_dbContext, contestId, currentUser.Id)
                && !await _userManager.IsInRoleAsync(currentUser, RolesContainer.Moderator))
            {
                _logger.LogWarning($"Попытка от пользователя с идентификатором {currentUser.Id} получить статистику " +
                    $"участника с идентификатором {userId} в рамках соревнования с идентификатором {contestId}, не имеющего прав на данное действие");

                return BadRequest(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
            }

            if (!await _contestsManager.IsUserContestParticipantAsync(_dbContext, contestId, userId))
            {
                _logger.LogWarning($"Попытка от пользователя с идентификатором {currentUser.Id} получить статистику " +
                    $"участника с идентификатором {userId} в рамках соревнования с идентификатором {contestId}, когда такого участника в соревновании нет");

                return NotFound(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserNotInContestErrorName]);
            }

            var contestParticipant = await _dbContext.ContestsParticipants.FirstOrDefaultAsync(cp => cp.ParticipantId == userId
                                                                                                        && cp.ContestId == contestId);

            var contestProblems = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == contestId)
                                                                    .ToListAsync();

            var solutions = await _dbContext.Solutions.Where(s => s.ContestId == contestId
                                                                    && s.ParticipantId == userId)
                                                        .ToListAsync();

            return Json(_contestsManager.GetMonitorEntryForParticipant(contestParticipant, contestProblems, solutions));
        }

        [HttpPost("{contestId}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddParticipant(long contestId, [FromBody] ParticipantForm participantForm)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);

            if (contestId != participantForm.ContestId)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} добавить участника с идентификатором {participantForm.UserId} с идентификатором {contestId}, когда в переданной форме указано соревнование с идентификатором {participantForm.ContestId}");
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
                    if (contest == null)
                    {
                        _logger.LogWarning(
                            $"Попытка от пользователя с идентификатором {currentUser.Id} добавить участника с идентификатором {participantForm.UserId} в несуществующее соревнование с идентификатором {contestId}");
                        response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == participantForm.UserId);
                        if (user == null)
                        {
                            _logger.LogWarning(
                                $"Попытка от пользователя {currentUser.Id} добавить несуществующего пользователя с идентификатором {participantForm.UserId} в качестве участника для соревнования с идентификатором {contestId}");
                            response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.EntityDoesntExistErrorName]);
                        }
                        else
                        {
                            if (await _dbContext.ContestsParticipants.AnyAsync(cp =>
                            cp.ContestId == contestId && cp.ParticipantId == participantForm.UserId))
                            {
                                _logger.LogWarning(
                                    $"Попытка от пользователя {currentUser.Id} добавить пользователя с идентификатором {participantForm.UserId} в качестве участника для соревнования с идентификатором {contestId}, когда данный пользователь уже является участником данного соревнования");
                                response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserAlreadyInContestErrorName]);
                            }
                            else
                            {
                                var contestParticipant = new ContestParticipant
                                {
                                    ParticipantId = participantForm.UserId,
                                    ContestId = participantForm.ContestId,
                                    Alias = participantForm.Alias,
                                    ConfirmedByParticipant = true, // TODO: сделать нормальные проверки на инвайты и прочую чепухню
                                    ConfirmedByLocalModerator = true,
                                    ConfirmingLocalModeratorId = contest.CreatorId
                                };
                                await _dbContext.ContestsParticipants.AddAsync(contestParticipant);
                                await _dbContext.SaveChangesAsync();

                                await _contestsManager.AddParticipantToChatsAsync(_dbContext, contestParticipant);
                                _logger.LogInformation(
                                    $"В соревнование с идентификатором {contest.Id} успешно добавлен участник с идентификатором {participantForm.UserId} пользователем с идентификатором {currentUser.Id}");
                                response = ResponseObject<long>.Success(contestId);
                            }
                        }
                    }
                }
                else
                {
                    response = ResponseObject<long>.Fail(ModelState, Constants.UserEntityName);
                }
            }
            return Json(response);
        }

        [HttpDelete("{contestId}/{userId}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> DeleteParticipant(long contestId, long userId)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null)
            {
                _logger.LogWarning(
                    $"Попытка от пользователя с идентификатором {currentUser.Id} удалить из списка участников несуществующего соревнования с идентификатором {contestId} пользователя с идентификатором {userId}");
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }
            else
            {
                var contestParticipant =
                await _dbContext.ContestsParticipants.FirstOrDefaultAsync(cp =>
                    cp.ContestId == contestId && cp.ParticipantId == userId);
                if (contestParticipant == null)
                {
                    _logger.LogWarning(
                        $"Попытка от пользователя с идентификатором {currentUser.Id} удалить из списка участников соревнования с идентификатором {contestId} несуществующего участника с идентификатором {userId}");
                    response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.EntityDoesntExistErrorName]);
                }
                else
                {
                    if (currentUser.Id != userId && !contest.ContestLocalModerators.Any(clm => clm.ContestId == contestId && clm.LocalModeratorId == currentUser.Id))
                    {
                        _logger.LogWarning(
                            $"Попытка от пользователя с идентификатором {currentUser.Id} удалить из списка участников соревнования с идентификатором {contestId} участника с идентификатором {userId}, не имея на это прав");
                        response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        await _contestsManager.RemoveParticipantFromChatsAsync(_dbContext, contestParticipant);
                        _dbContext.ContestsParticipants.Remove(contestParticipant);
                        await _dbContext.SaveChangesAsync();
                        _logger.LogInformation(
                            $"Пользователем с идентификатором {currentUser.Id} из списка участников соревнования с идентификатором {contestId} успешно удалён пользователь с идентификатором {userId}");
                        response = ResponseObject<long>.Success(userId);
                    }
                }
            }
            return Json(response);
        }
    }
}
