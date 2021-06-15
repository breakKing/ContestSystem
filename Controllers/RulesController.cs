using ContestSystemDbStructure.Models;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
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
    public class RulesController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<RulesController> _logger;

        public RulesController(MainDbContext dbContext, ILogger<RulesController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("get-user-rules/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserRules(long id)
        {
            var rules = await _dbContext.RulesSets.Where(r => r.AuthorId == id && !r.IsArchieved).ToListAsync();
            var publishedRules = rules.ConvertAll(r =>
            {
                var pr = ConstructedRulesSet.GetFromModel(r);
                return pr;
            });
            return Json(publishedRules);
        }

        [HttpGet("get-available-rules/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetAvailableRules(long id)
        {
            var rules = await _dbContext.RulesSets.Where(rs => (rs.AuthorId == id || rs.IsPublic)
                                                                && !rs.IsArchieved).ToListAsync();
            var publishedRules = rules.ConvertAll(r =>
            {
                var pr = ConstructedRulesSet.GetFromModel(r);
                return pr;
            });
            return Json(publishedRules);
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetConstructedRules(long id)
        {
            var rules = await _dbContext.RulesSets.FirstOrDefaultAsync(r => r.Id == id && !r.IsArchieved);
            if (rules != null)
            {
                var publishedRules = ConstructedRulesSet.GetFromModel(rules);
                return Json(publishedRules);
            }
            return NotFound("Такого набора правил не существует");
        }

        [HttpPost("add-rules")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddRules([FromBody] RulesSetForm rulesSetForm)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await HttpContext.GetCurrentUser();
                if (currentUser.Id != rulesSetForm.AuthorId)
                {
                    _logger.LogCreationByNonEqualCurrentUserAndCreator("RulesSet", currentUser.Id, rulesSetForm.AuthorId);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Id автора в форме отличается от Id текущего пользователя" }
                    });
                }
                var rules = new RulesSet
                {
                    Name = rulesSetForm.Name,
                    Description = rulesSetForm.Description,
                    ShowFullTestsResults = rulesSetForm.ShowFullTestsResults,
                    PointsForBestSolution = rulesSetForm.PointsForBestSolution,
                    CountMode = rulesSetForm.CountMode,
                    MaxTriesForOneProblem = rulesSetForm.MaxTriesForOneProblem,
                    PenaltyForOneMinute = rulesSetForm.PenaltyForOneMinute,
                    MonitorFreezeTimeBeforeFinishInMinutes = rulesSetForm.MonitorFreezeTimeBeforeFinishInMinutes,
                    PenaltyForCompilationError = rulesSetForm.PenaltyForCompilationError,
                    PenaltyForOneTry = rulesSetForm.PenaltyForOneTry,
                    PublicMonitor = rulesSetForm.PublicMonitor,
                    AuthorId = rulesSetForm.AuthorId,
                    IsPublic = rulesSetForm.IsPublic,
                    IsArchieved = false
                };
                await _dbContext.RulesSets.AddAsync(rules);
                await _dbContext.SaveChangesAsync();
                _logger.LogCreationSuccessful("RulesSet", rules.Id, currentUser.Id);
                return Json(new
                {
                    status = true,
                    data = rules.Id,
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

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpPut("edit-rules/{id}")]
        public async Task<IActionResult> EditRules([FromBody] RulesSetForm rulesSetForm, long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (rulesSetForm.Id == null || id <= 0 || id != rulesSetForm.Id)
            {
                _logger.LogEditingWithNonEqualFormAndRequestId("RulesSet", rulesSetForm.Id, id, currentUser.Id);
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Id в запросе не совпадает с Id в форме" }
                });
            }

            if (ModelState.IsValid)
            {
                var rules = await _dbContext.RulesSets.FirstOrDefaultAsync(rs => rs.Id == id && !rs.IsArchieved);
                if (rules == null)
                {
                    _logger.LogEditingOfNonExistentEntity("RulesSet", id, currentUser.Id);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Попытка изменить несуществующий набор правил" }
                    });
                }
                else
                {
                    if (currentUser.Id != rules.AuthorId)
                    {
                        _logger.LogEditingByNotAppropriateUser("RulesSet", id, currentUser.Id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Попытка изменить не свой набор правил" }
                        });
                    }
                    rules.Name = rulesSetForm.Name;
                    rules.Description = rulesSetForm.Description;
                    rules.ShowFullTestsResults = rulesSetForm.ShowFullTestsResults;
                    rules.PointsForBestSolution = rulesSetForm.PointsForBestSolution;
                    rules.CountMode = rulesSetForm.CountMode;
                    rules.MaxTriesForOneProblem = rulesSetForm.MaxTriesForOneProblem;
                    rules.PenaltyForOneMinute = rulesSetForm.PenaltyForOneMinute;
                    rules.MonitorFreezeTimeBeforeFinishInMinutes = rulesSetForm.MonitorFreezeTimeBeforeFinishInMinutes;
                    rules.PenaltyForCompilationError = rulesSetForm.PenaltyForCompilationError;
                    rules.PenaltyForOneTry = rulesSetForm.PenaltyForOneTry;
                    rules.PublicMonitor = rulesSetForm.PublicMonitor;
                    rules.IsPublic = rulesSetForm.IsPublic;
                    _dbContext.RulesSets.Update(rules);
                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _logger.LogParallelSaveError("RulesSet", id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Ошибка параллельного сохранения" }
                        });
                    }
                    _logger.LogEditingSuccessful("RulesSet", id, currentUser.Id);
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

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpDelete("delete-rules/{id}")]
        public async Task<IActionResult> DeleteRules(long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var loadedRules = await _dbContext.RulesSets.FirstOrDefaultAsync(rs => rs.Id == id && !rs.IsArchieved);
            if (loadedRules == null)
            {
                _logger.LogDeletingOfNonExistentEnitiy("RulesSet", id, currentUser.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Попытка удалить несуществующий набор правил" }
                });
            }
            var moderatorRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RolesContainer.Moderator);
            if (currentUser.Id != loadedRules.AuthorId && !currentUser.Roles.Contains(moderatorRole))
            {
                _logger.LogDeletingByNotAppropriateUser("RulesSet", id, currentUser.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Попытка удалить не свой набор правил" }
                });
            }
            if (await _dbContext.Contests.AnyAsync(c => c.RulesSetId == id))
            {
                loadedRules.IsArchieved = true;
                _dbContext.RulesSets.Update(loadedRules);
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
                        loadedRules = await _dbContext.RulesSets.FirstOrDefaultAsync(rs => rs.Id == id && !rs.IsArchieved);
                        if (loadedRules == null)
                        {
                            break;
                        }
                        loadedRules.IsArchieved = true;
                        _dbContext.RulesSets.Update(loadedRules);
                    }
                    _logger.LogDeletingByArchieving("RulesSet", id, currentUser.Id);
                }
            }
            else
            {
                _dbContext.RulesSets.Remove(loadedRules);
                await _dbContext.SaveChangesAsync();
                _logger.LogDeletingSuccessful("RulesSet", id, currentUser.Id);
            }
            return Json(new
            {
                status = true,
                errors = new List<string>()
            });
        }
    }
}
