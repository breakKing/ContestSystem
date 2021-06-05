using ContestSystemDbStructure.Models;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RulesController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public RulesController(MainDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("get-user-rules/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserRules(long id)
        {
            var rules = await _dbContext.RulesSets.Where(r => r.AuthorId == id).ToListAsync();
            var publishedRules = rules.ConvertAll(r =>
            {
                var pr = new ConstructedRulesSet
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    ShowFullTestsResults = r.ShowFullTestsResults,
                    PointsForBestSolution = r.PointsForBestSolution,
                    CountMode = r.CountMode,
                    MaxTriesForOneProblem = r.MaxTriesForOneProblem,
                    PenaltyForOneMinute = r.PenaltyForOneMinute,
                    MonitorFreezeTimeBeforeFinishInMinutes = r.MonitorFreezeTimeBeforeFinishInMinutes,
                    PenaltyForCompilationError = r.PenaltyForCompilationError,
                    PenaltyForOneTry = r.PenaltyForOneTry,
                    PublicMonitor = r.PublicMonitor,
                    Author = r.Author?.ResponseStructure,
                    IsPublic = r.IsPublic
                };
                return pr;
            });
            return Json(publishedRules);
        }

        [HttpGet("get-available-rules/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetAvailableRules(long id)
        {
            var rules = await _dbContext.RulesSets.Where(rs => rs.AuthorId == id || rs.IsPublic).ToListAsync();
            var publishedRules = rules.ConvertAll(r =>
            {
                var pr = new ConstructedRulesSet
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    ShowFullTestsResults = r.ShowFullTestsResults,
                    PointsForBestSolution = r.PointsForBestSolution,
                    CountMode = r.CountMode,
                    MaxTriesForOneProblem = r.MaxTriesForOneProblem,
                    PenaltyForOneMinute = r.PenaltyForOneMinute,
                    MonitorFreezeTimeBeforeFinishInMinutes = r.MonitorFreezeTimeBeforeFinishInMinutes,
                    PenaltyForCompilationError = r.PenaltyForCompilationError,
                    PenaltyForOneTry = r.PenaltyForOneTry,
                    PublicMonitor = r.PublicMonitor,
                    Author = r.Author?.ResponseStructure,
                    IsPublic = r.IsPublic
                };
                return pr;
            });
            return Json(publishedRules);
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetConstructedRules(long id)
        {
            var rules = await _dbContext.RulesSets.FirstOrDefaultAsync(r => r.Id == id);
            if (rules != null)
            {
                var publishedRules = new ConstructedRulesSet
                {
                    Id = rules.Id,
                    Name = rules.Name,
                    Description = rules.Description,
                    ShowFullTestsResults = rules.ShowFullTestsResults,
                    PointsForBestSolution = rules.PointsForBestSolution,
                    CountMode = rules.CountMode,
                    MaxTriesForOneProblem = rules.MaxTriesForOneProblem,
                    PenaltyForOneMinute = rules.PenaltyForOneMinute,
                    MonitorFreezeTimeBeforeFinishInMinutes = rules.MonitorFreezeTimeBeforeFinishInMinutes,
                    PenaltyForCompilationError = rules.PenaltyForCompilationError,
                    PenaltyForOneTry = rules.PenaltyForOneTry,
                    PublicMonitor = rules.PublicMonitor,
                    Author = rules.Author?.ResponseStructure,
                    IsPublic = rules.IsPublic
                };
                return Json(publishedRules);
            }
            return Json(new
            {
                status = false,
                errors = new List<string> { "Набора правил с таким идентификатором не существует" }
            });
        }

        [HttpPost("add-rules")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddRules([FromBody] RulesSetForm rulesSetForm)
        {
            if (ModelState.IsValid)
            {
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
                    IsPublic = rulesSetForm.IsPublic
                };
                await _dbContext.RulesSets.AddAsync(rules);
                await _dbContext.SaveChangesAsync();
                return Json(new
                {
                    status = true,
                    message = ""
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
            if (rulesSetForm.Id == null || id <= 0 || id != rulesSetForm.Id)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Id в запросе не совпадает с Id в форме" }
                });
            }

            if (ModelState.IsValid)
            {
                var rules = await _dbContext.RulesSets.FirstOrDefaultAsync(rs => rs.Id == id);
                if (rules == null)
                {
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Попытка изменить несуществующий набор правил" }
                    });
                }
                else
                {
                    if ((await HttpContext.GetCurrentUser()).Id != rules.AuthorId)
                    {
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
                    rules.AuthorId = rulesSetForm.AuthorId;
                    rules.IsPublic = rulesSetForm.IsPublic;
                    _dbContext.RulesSets.Update(rules);
                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Ошибка параллельного сохранения" }
                        });
                    }

                    return Json(new
                    {
                        status = true,
                        message = ""
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
            var loadedRules = await _dbContext.RulesSets.FindAsync(id);
            if (loadedRules == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Попытка удалить несуществующий набор правил" }
                });
            }

            var currentUser = await HttpContext.GetCurrentUser();
            var moderatorRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RolesContainer.Moderator);
            if (currentUser.Id != loadedRules.AuthorId && !currentUser.Roles.Contains(moderatorRole))
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Попытка удалить не свою задачу или без модераторских прав" }
                });
            }

            _dbContext.RulesSets.Remove(loadedRules);
            await _dbContext.SaveChangesAsync();
            return Json(new
            {
                status = true,
                message = ""
            });
        }
    }
}
