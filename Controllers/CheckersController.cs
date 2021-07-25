using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using ContestSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckersController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly CheckerSystemService _checkerSystemService;
        private readonly ILogger<CheckersController> _logger;

        public CheckersController(MainDbContext dbContext, CheckerSystemService checkerSystemService,
            ILogger<CheckersController> logger)
        {
            _dbContext = dbContext;
            _checkerSystemService = checkerSystemService;
            _logger = logger;
        }

        [HttpGet("get-user-checkers/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserCheckers(long id)
        {
            var checkers = await _dbContext.Checkers.Where(p => p.AuthorId == id && !p.IsArchieved).ToListAsync();
            var publishedCheckers = checkers.ConvertAll(c =>
            {
                var pc = PublishedChecker.GetFromModel(c);
                return pc;
            });
            return Json(publishedCheckers);
        }

        [HttpGet("get-available-checkers/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetAvailableCheckers(long id)
        {
            var checkers = await _dbContext.Checkers.Where(c => (c.AuthorId == id || c.IsPublic)
                                                                && !c.IsArchieved
                                                                && c.ApprovalStatus == ApproveType.Accepted)
                .ToListAsync();
            var publishedCheckers = checkers.ConvertAll(c =>
            {
                var pc = PublishedChecker.GetFromModel(c);
                return pc;
            });
            return Json(publishedCheckers);
        }

        [HttpGet("published/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetPublishedChecker(long id)
        {
            var checker = await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id && !ch.IsArchieved);
            if (checker == null)
            {
                return NotFound("Такого чекера не существует");
            }

            var publishedChecker = PublishedChecker.GetFromModel(checker);
            return Json(publishedChecker);
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetConstructedChecker(long id)
        {
            var checker = await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id && !ch.IsArchieved);
            if (checker == null)
            {
                return NotFound("Такого чекера не существует");
            }

            var constructedChecker = ConstructedChecker.GetFromModel(checker);
            return Json(constructedChecker);
        }

        [HttpPost("add-checker")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddChecker([FromBody] CheckerForm checkerForm)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (ModelState.IsValid)
            {
                var checker = new Checker
                {
                    AuthorId = checkerForm.AuthorId,
                    Code = checkerForm.Code,
                    Name = checkerForm.Name,
                    Description = checkerForm.Description,
                    IsPublic = checkerForm.IsPublic,
                    IsArchieved = false
                };
                if (currentUser.Id != checkerForm.AuthorId)
                {
                    _logger.LogCreationByNonEqualCurrentUserAndCreator("Checker", currentUser.Id, checkerForm.AuthorId);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Id автора в форме отличается от Id текущего пользователя"}
                    });
                }

                checker.ApprovalStatus = ApproveType.NotModeratedYet;
                await _dbContext.Checkers.AddAsync(checker);
                await _dbContext.SaveChangesAsync();
                _logger.LogCreationSuccessful("Checker", checker.Id, currentUser.Id);
                return Json(new
                {
                    status = true,
                    data = checker.Id,
                    errors = new List<string>()
                });
            }

            return Json(new
            {
                status = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }

        [HttpPut("edit-checker/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> EditChecker([FromBody] CheckerForm checkerForm, long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (checkerForm.Id == null || checkerForm.Id.Value != id)
            {
                _logger.LogEditingWithNonEqualFormAndRequestId("Checker", checkerForm.Id, id, currentUser.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Id в форме не совпадает с Id в запросе"}
                });
            }

            if (ModelState.IsValid)
            {
                var checker = await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id);
                if (checker == null)
                {
                    _logger.LogEditingOfNonExistentEntity("Checker", id, currentUser.Id);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Такого чекера не существует"}
                    });
                }

                if (checker.AuthorId != currentUser.Id)
                {
                    _logger.LogEditingByNotAppropriateUser("Checker", id, currentUser.Id);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Попытка изменить не свой чекер"}
                    });
                }

                bool needToRecompile = (checker.Code != checkerForm.Code);
                checker.Code = checkerForm.Code;
                checker.Name = checkerForm.Name;
                checker.Description = checkerForm.Description;
                checker.IsPublic = checkerForm.IsPublic;
                if (needToRecompile || checker.ApprovalStatus == ApproveType.Rejected)
                {
                    checker.ApprovalStatus = ApproveType.NotModeratedYet;
                    checker.ApprovingModeratorId = null;
                }

                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    _logger.LogParallelSaveError("Checker", id);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Ошибка параллельного сохранения"}
                    });
                }

                _logger.LogInformation(
                    $"Механизм проверки с идентификатором {id} успешно изменён и {(needToRecompile ? "" : "не ")}нуждается в модерации и компиляции");
                return Json(new
                {
                    status = true,
                    errors = new List<string>()
                });
            }

            return Json(new
            {
                status = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }

        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        [HttpDelete("delete-checker/{id}")]
        public async Task<IActionResult> DeleteChecker(long id)
        {
            Checker loadedChecker = await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id && !ch.IsArchieved);
            var currentUser = await HttpContext.GetCurrentUser();
            if (loadedChecker == null)
            {
                _logger.LogDeletingOfNonExistentEnitiy("Checker", id, currentUser.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить несуществующий чекер"}
                });
            }

            var moderatorRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RolesContainer.Moderator);
            if (currentUser.Id != loadedChecker.AuthorId && !currentUser.Roles.Contains(moderatorRole))
            {
                _logger.LogDeletingByNotAppropriateUser("Checker", id, currentUser.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить не свой чекер или без модераторских прав"}
                });
            }

            if (await _dbContext.Problems.AnyAsync(p => p.CheckerId == id))
            {
                loadedChecker.IsArchieved = true;
                _dbContext.Checkers.Update(loadedChecker);
                bool saved = false;
                while (!saved)
                {
                    try
                    {
                        await _dbContext.SaveChangesAsync();
                        saved = true;
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        loadedChecker =
                            await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id && !ch.IsArchieved);
                        if (loadedChecker == null)
                        {
                            break;
                        }

                        loadedChecker.IsArchieved = true;
                        _dbContext.Checkers.Update(loadedChecker);
                    }

                    _logger.LogDeletingByArchiving("Checker", id, currentUser.Id);
                }
            }
            else
            {
                _dbContext.Checkers.Remove(loadedChecker);
                await _dbContext.SaveChangesAsync();
                _logger.LogDeletingSuccessful("Checker", id, currentUser.Id);
            }

            return Json(new
            {
                status = true,
                errors = new List<string>()
            });
        }


        [HttpGet("get-requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetCheckersRequests()
        {
            var checkers = await _dbContext.Checkers
                .Where(c => c.ApprovalStatus == ApproveType.NotModeratedYet && !c.IsArchieved).ToListAsync();
            var requests = checkers.ConvertAll(c =>
            {
                ConstructedChecker cr = ConstructedChecker.GetFromModel(c);
                return cr;
            });
            return Json(requests);
        }

        [HttpGet("get-approved")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetApprovedCheckers()
        {
            var checkers = await _dbContext.Checkers
                .Where(c => c.ApprovalStatus == ApproveType.Accepted && !c.IsArchieved).ToListAsync();
            var requests = checkers.ConvertAll(c =>
            {
                ConstructedChecker cr = ConstructedChecker.GetFromModel(c);
                return cr;
            });
            return Json(requests);
        }

        [HttpGet("get-rejected")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetRejectedCheckers()
        {
            var checkers = await _dbContext.Checkers
                .Where(c => c.ApprovalStatus == ApproveType.Rejected && !c.IsArchieved).ToListAsync();
            var requests = checkers.ConvertAll(c =>
            {
                ConstructedChecker cr = ConstructedChecker.GetFromModel(c);
                return cr;
            });
            return Json(requests);
        }

        [HttpPut("moderate/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ApproveOrRejectChecker([FromBody] CheckerRequestForm checkerRequestForm,
            long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (checkerRequestForm.CheckerId != id || id < 0)
            {
                _logger.LogModeratingWithNonEqualFormAndRequestId("Checker", checkerRequestForm.CheckerId, id,
                    currentUser.Id);
                return Json(new
                {
                    success = false,
                    errors = new List<string> {"Id в запросе не совпадает с Id в форме"}
                });
            }

            if (ModelState.IsValid)
            {
                var checker = await _dbContext.Checkers.FirstOrDefaultAsync(c => c.Id == id && !c.IsArchieved);
                if (checker == null)
                {
                    _logger.LogModeratingOfNonExistentEntity("Checker", id, currentUser.Id);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Попытка модерировать несуществующий чекер"}
                    });
                }
                else
                {
                    checker.ApprovalStatus = checkerRequestForm.ApprovalStatus;
                    checker.ApprovingModeratorId = checkerRequestForm.ApprovingModeratorId;
                    checker.ModerationMessage = checkerRequestForm.ModerationMessage;
                    if (checker.ApprovalStatus == ApproveType.Accepted)
                    {
                        _logger.LogInformation(
                            $"Отправлена на компиляцию сущность \"Checker\" с идентификатором {id} модератором с идентификатором {currentUser.Id}");
                        var newChecker = await _checkerSystemService.SendCheckerForCompilationAsync(checker);
                        checker.CompilationVerdict = newChecker.CompilationVerdict;
                        checker.Errors = newChecker.Errors;
                        if (newChecker.CompilationVerdict != VerdictType.CompilationSucceed)
                        {
                            _logger.LogInformation(
                                $"В результате компиляции \"Checker\" с идентификатором {id} возникли ошибки");
                            checker.ApprovalStatus = ApproveType.Rejected;
                            checker.ModerationMessage = "Compilation errors:\n" + newChecker.Errors;
                        }
                    }

                    _dbContext.Checkers.Update(checker);
                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _logger.LogParallelSaveError("Checker", id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> {"Ошибка параллельного сохранения"}
                        });
                    }

                    _logger.LogModeratingSuccessful("Checker", id, currentUser.Id, checkerRequestForm.ApprovalStatus);
                    return Json(new
                    {
                        status = true,
                        errors = new List<string>()
                    });
                }
            }

            return Json(new
            {
                status = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }
    }
}