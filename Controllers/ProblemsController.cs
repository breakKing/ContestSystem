using ContestSystemDbStructure.Enums;
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
using ContestSystemDbStructure.Models.common;
using Microsoft.Extensions.Logging;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProblemsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<ProblemsController> _logger;

        public ProblemsController(MainDbContext dbContext, ILogger<ProblemsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("get-user-problems/{id}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserProblems(long id, string culture)
        {
            var problems = await _dbContext.Problems.Where(p => p.CreatorId == id).ToListAsync();
            var publishedProblems = problems.ConvertAll(p =>
            {
                var localizer = p.ProblemLocalizers.FirstOrDefault(pl => pl.Culture == culture);
                var pp = PublishedProblem.GetFromModel(p, localizer);
                return pp;
            });
            return Json(publishedProblems);
        }

        [HttpGet("get-available-problems/{id}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetAvailableProblems(long id, string culture)
        {
            var problems = await _dbContext.Problems.Where(p => (p.CreatorId == id || p.IsPublic)
                                                                    && p.ApprovalStatus == ApproveType.Accepted)
                                                    .ToListAsync();
            var publishedProblems = problems.ConvertAll(p =>
            {
                var localizer = p.ProblemLocalizers.FirstOrDefault(pl => pl.Culture == culture);
                var pp = PublishedProblem.GetFromModel(p, localizer);
                return pp;
            });
            return Json(publishedProblems);
        }

        [HttpGet("published/{id}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetPublishedProblem(long id, string culture)
        {
            var problem = await _dbContext.Problems.FirstOrDefaultAsync(p => p.Id == id);
            if (problem != null)
            {
                var localizer = problem.ProblemLocalizers.FirstOrDefault(pl => pl.Culture == culture);
                if (localizer == null)
                {
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Такой локализации под данную задачу не существует" }
                    });
                }
                var publishedProblem = PublishedProblem.GetFromModel(problem, localizer);
                return Json(publishedProblem);
            }
            return Json(new
            {
                status = false,
                errors = new List<string> { "Задачи с таким идентификатором не существует" }
            });
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetConstructedProblem(long id)
        {
            var problem = await _dbContext.Problems.FirstOrDefaultAsync(p => p.Id == id);
            if (problem != null)
            {
                var constructedProblem = ConstructedProblem.GetFromModel(problem);
                return Json(constructedProblem);
            }
            return Json(new
            {
                status = false,
                errors = new List<string> { "Задачи с таким идентификатором не существует" }
            });
        }

        [HttpPost("add-problem")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddProblem([FromBody] ProblemForm problemForm)
        {
            if (problemForm.Tests.Sum(t => t.AvailablePoints) != 100)
            {
                ModelState.AddModelError("Tests", "Sum of available points for all tests is not equal to 100");
            }
            if (ModelState.IsValid)
            {
                var currentUser = await HttpContext.GetCurrentUser();
                if (!await _dbContext.Checkers.AnyAsync(ch => ch.Id == problemForm.CheckerId))
                {
                    _logger.LogWarning($"Попытка от пользователя с идентификатором {currentUser.Id} создать сущность \"Problem\" с использованием несуществующей сущности \"Checker\" с идентификатором {problemForm.CheckerId}");
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Нужного чекера не существует" }
                    });
                }
                var problem = new Problem
                {
                    CreatorId = problemForm.CreatorId,
                    IsPublic = problemForm.IsPublic,
                    MemoryLimitInBytes = problemForm.MemoryLimitInBytes,
                    TimeLimitInMilliseconds = problemForm.TimeLimitInMilliseconds,
                    CheckerId = problemForm.CheckerId
                };
                if (problemForm.CreatorId != currentUser.Id)
                {
                    _logger.LogCreationByNonEqualCurrentUserAndCreator("Problem", currentUser.Id, problemForm.CreatorId);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Id автора в форме отличается от Id текущего пользователя" }
                    });
                }
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == problemForm.CreatorId);
                if (user == null)
                {
                    _logger.LogCreationByNonExistentUser("Problem", problemForm.CreatorId);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Автор является несуществующим пользователем" }
                    });
                }
                if (user.IsLimitedInProblems)
                {
                    if (await _dbContext.Problems.CountAsync(c => c.CreatorId == user.Id) == 1)
                    {
                        _logger.LogCreationFailedBecauseOfLimits("Problem", currentUser.Id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Превышено ограничение недоверенного пользователя по созданию задач" }
                        });
                    }
                    problem.ApprovalStatus = ApproveType.NotModeratedYet;
                }
                else
                {
                    problem.ApprovalStatus = ApproveType.Accepted;
                }
                problem.ApprovalStatus = ApproveType.Accepted;
                await _dbContext.Problems.AddAsync(problem);
                await _dbContext.SaveChangesAsync();
                foreach (var localizer in problemForm.Localizers)
                {
                    var problemLocalizer = new ProblemLocalizer
                    {
                        Culture = localizer.Culture,
                        Description = localizer.Description,
                        Name = localizer.Name,
                        InputBlock = localizer.InputBlock,
                        OutputBlock = localizer.OutputBlock,
                        ProblemId = problem.Id
                    };
                    await _dbContext.ProblemsLocalizers.AddAsync(problemLocalizer);
                }
                foreach (var test in problemForm.Tests)
                {
                    var problemTest = new Test
                    {
                        Number = test.Number,
                        Input = test.Input,
                        Answer = test.Answer,
                        AvailablePoints = test.AvailablePoints,
                        ProblemId = problem.Id
                    };
                    await _dbContext.Tests.AddAsync(problemTest);
                }
                foreach (var example in problemForm.Examples)
                {
                    var problemExample = new Example
                    {
                        Number = example.Number,
                        InputText = example.InputText,
                        OutputText = example.OutputText,
                        ProblemId = problem.Id
                    };
                    await _dbContext.Examples.AddAsync(problemExample);
                }
                await _dbContext.SaveChangesAsync();
                if (problem.ApprovalStatus == ApproveType.Accepted)
                {
                    _logger.LogCreationSuccessfulWithAutoAccept("Problem", problem.Id, currentUser.Id);
                }
                else
                {
                    _logger.LogCreationSuccessful("Problem", problem.Id, currentUser.Id);
                }
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

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpPut("edit-problem/{id}")]
        public async Task<IActionResult> EditProblem([FromBody] ProblemForm problemForm, long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (problemForm.Id == null || id <= 0 || id != problemForm.Id)
            {
                _logger.LogEditingWithNonEqualFormAndRequestId("Problem", problemForm.Id, id, currentUser.Id);
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Id в запросе не совпадает с Id в форме" }
                });
            }
            if (problemForm.Tests.Sum(t => t.AvailablePoints) != 100)
            {
                ModelState.AddModelError("Tests", "Sum of available points for all tests is not equal to 100");
            }
            if (ModelState.IsValid)
            {
                var problem = await _dbContext.Problems.FirstOrDefaultAsync(p => p.Id == id);
                if (problem == null)
                {
                    _logger.LogEditingOfNonExistentEntity("Problem", id, currentUser.Id);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Попытка изменить несуществующую задачу" }
                    });
                }
                else
                {
                    if (currentUser.Id != problem.CreatorId)
                    {
                        _logger.LogEditingByNotAppropriateUser("Problem", id, currentUser.Id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Попытка изменить не свою задачу" }
                        });
                    }
                    
                    problem.MemoryLimitInBytes = problemForm.MemoryLimitInBytes;
                    problem.TimeLimitInMilliseconds = problemForm.TimeLimitInMilliseconds;
                    problem.IsPublic = problemForm.IsPublic;
                    problem.CheckerId = problemForm.CheckerId;
                    if (problem.ApprovalStatus == ApproveType.Rejected)
                    {
                        problem.ApprovalStatus = ApproveType.NotModeratedYet;
                        problem.ApprovingModeratorId = null;
                    }
                    _dbContext.Problems.Update(problem);
                    for (int i = 0; i < problemForm.Localizers.Count; i++)
                    {
                        var localizer = new ProblemLocalizer
                        {
                            Culture = problemForm.Localizers[i].Culture,
                            Description = problemForm.Localizers[i].Description,
                            InputBlock = problemForm.Localizers[i].InputBlock,
                            OutputBlock = problemForm.Localizers[i].OutputBlock,
                            Name = problemForm.Localizers[i].Name,
                            ProblemId = problem.Id
                        };
                        var loadedLocalizer = await _dbContext.ProblemsLocalizers.FirstOrDefaultAsync(pl => pl.Culture == localizer.Culture && pl.ProblemId == id);
                        if (loadedLocalizer == null)
                        {
                            await _dbContext.ProblemsLocalizers.AddAsync(localizer);
                        }
                        else
                        {
                            loadedLocalizer.Description = localizer.Description;
                            loadedLocalizer.Name = localizer.Name;
                            loadedLocalizer.InputBlock = localizer.InputBlock;
                            loadedLocalizer.OutputBlock = localizer.OutputBlock;
                            _dbContext.ProblemsLocalizers.Update(loadedLocalizer);
                        }
                    }
                    for (int i = 0; i < problemForm.Tests.Count; i++)
                    {
                        var test = new Test
                        {
                            Number = problemForm.Tests[i].Number,
                            Input = problemForm.Tests[i].Input,
                            Answer = problemForm.Tests[i].Answer,
                            AvailablePoints = problemForm.Tests[i].AvailablePoints,
                            ProblemId = problem.Id
                        };
                        var loadedTest = await _dbContext.Tests.FirstOrDefaultAsync(t => t.Number == test.Number && t.ProblemId == id);
                        if (loadedTest == null)
                        {
                            await _dbContext.Tests.AddAsync(test);
                        }
                        else
                        {
                            loadedTest.Number = test.Number;
                            loadedTest.AvailablePoints = test.AvailablePoints;
                            loadedTest.Input = test.Input;
                            loadedTest.Answer = test.Answer;
                            _dbContext.Tests.Update(loadedTest);
                        }
                    }
                    for (int i = 0; i < problemForm.Examples.Count; i++)
                    {
                        var example = new Example
                        {
                            Number = problemForm.Examples[i].Number,
                            InputText = problemForm.Examples[i].InputText,
                            OutputText = problemForm.Examples[i].OutputText,
                            ProblemId = problem.Id
                        };
                        var loadedExample = await _dbContext.Examples.FirstOrDefaultAsync(e => e.Number == example.Number && e.ProblemId == id);
                        if (loadedExample == null)
                        {
                            await _dbContext.Examples.AddAsync(example);
                        }
                        else
                        {
                            loadedExample.Number = example.Number;
                            loadedExample.InputText = example.InputText;
                            loadedExample.OutputText = example.OutputText;
                            _dbContext.Examples.Update(loadedExample);
                        }
                    }
                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _logger.LogParallelSaveError("Problem", id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Ошибка параллельного сохранения" }
                        });
                    }
                    _logger.LogEditingSuccessful("Problem", id, currentUser.Id);
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
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        [HttpDelete("delete-problem/{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            var loadedProblem = await _dbContext.Problems.FindAsync(id);
            if (loadedProblem == null)
            {
                _logger.LogDeletingOfNonExistentEnitiy("Problem", id, currentUser.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Попытка удалить несуществующую задачу" }
                });
            }

            var moderatorRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RolesContainer.Moderator);
            if (currentUser.Id != loadedProblem.CreatorId && !currentUser.Roles.Contains(moderatorRole))
            {
                _logger.LogDeletingByNotAppropriateUser("Problem", id, currentUser.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Попытка удалить не свою задачу или без модераторских прав" }
                });
            }

            _dbContext.Problems.Remove(loadedProblem);
            await _dbContext.SaveChangesAsync();
            _logger.LogDeletingSuccessful("Problem", id, currentUser.Id);
            return Json(new
            {
                status = true,
                errors = new List<string>()
            });
        }

        [HttpGet("get-requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetProblemsRequests()
        {
            var problems = await _dbContext.Problems.Where(p => p.ApprovalStatus == ApproveType.NotModeratedYet).ToListAsync();
            var requests = problems.ConvertAll(p =>
            {
                var pr = ConstructedProblem.GetFromModel(p);
                return pr;
            });
            return Json(requests);
        }

        [HttpGet("get-approved")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetApprovedProblems()
        {
            var problems = await _dbContext.Problems.Where(p => p.ApprovalStatus == ApproveType.Accepted).ToListAsync();
            var requests = problems.ConvertAll(p =>
            {
                var pr = ConstructedProblem.GetFromModel(p);
                return pr;
            });
            return Json(requests);
        }

        [HttpGet("get-rejected")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetRejectedProblems()
        {
            var problems = await _dbContext.Problems.Where(p => p.ApprovalStatus == ApproveType.Rejected).ToListAsync();
            var requests = problems.ConvertAll(p =>
            {
                var pr = ConstructedProblem.GetFromModel(p);
                return pr;
            });
            return Json(requests);
        }

        [HttpPut("moderate/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ApproveOrRejectProblem([FromBody] ProblemRequestForm problemRequestForm, long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (problemRequestForm.ProblemId != id || id < 0)
            {
                _logger.LogModeratingWithNonEqualFormAndRequestId("Problem", problemRequestForm.ProblemId, id, currentUser.Id);
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Id в запросе не совпадает с Id в форме" }
                });
            }

            if (ModelState.IsValid)
            {
                var problem = await _dbContext.Problems.FirstOrDefaultAsync(c => c.Id == id);
                if (problem == null)
                {
                    _logger.LogModeratingOfNonExistentEntity("Problem", id, currentUser.Id);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Попытка модерировать несуществующий пост" }
                    });
                }
                else
                {
                    problem.ApprovalStatus = problemRequestForm.ApprovalStatus;
                    problem.ApprovingModeratorId = problemRequestForm.ApprovingModeratorId;
                    problem.ModerationMessage = problemRequestForm.ModerationMessage;
                    _dbContext.Problems.Update(problem);
                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _logger.LogParallelSaveError("Problem", id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Ошибка параллельного сохранения" }
                        });
                    }
                    _logger.LogModeratingSuccessful("Problem", id, currentUser.Id, problemRequestForm.ApprovalStatus);
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
