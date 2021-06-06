using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using ContestSystem.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContestsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly CheckerSystemService _checkerSystemService;

        public ContestsController(MainDbContext dbContext, UserManager<User> userManager, CheckerSystemService checkerSystemService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _checkerSystemService = checkerSystemService;
        }

        [HttpGet("get-available-contests/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetAvailableContests(string culture)
        {
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                            && c.StartDateTimeUTC > DateTime.UtcNow
                                                            && c.IsPublic)
                                                    .ToListAsync();
            var localizers = contests.ConvertAll(c => c.ContestLocalizers.FirstOrDefault(cl => cl.Culture == culture));
            var publishedContests = new List<PublishedContest>();
            for (int i = 0; i < contests.Count; i++)
            {
                publishedContests.Add(new PublishedContest
                {
                    Id = contests[i].Id,
                    Creator = contests[i].Creator?.ResponseStructure,
                    LocalizedDescription = localizers[i].Description,
                    LocalizedName = localizers[i].Name,
                    StartDateTimeUTC = contests[i].StartDateTimeUTC,
                    EndDateTimeUTC = contests[i].EndDateTimeUTC,
                    Image = contests[i].Image,
                    ParticipantsCount = await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id),
                    ApprovalStatus = contests[i].ApprovalStatus
                });
            }
            return Json(publishedContests);
        }

        [HttpGet("get-running-contests/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetRunningContests(string culture)
        {
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                            && c.StartDateTimeUTC <= DateTime.UtcNow
                                                            && c.EndDateTimeUTC > DateTime.UtcNow
                                                            && c.RulesSet.PublicMonitor)
                                                    .ToListAsync();
            var localizers = contests.ConvertAll(c => c.ContestLocalizers.FirstOrDefault(cl => cl.Culture == culture));
            var publishedContests = new List<PublishedContest>();
            for (int i = 0; i < contests.Count; i++)
            {
                publishedContests.Add(new PublishedContest
                {
                    Id = contests[i].Id,
                    Creator = contests[i].Creator?.ResponseStructure,
                    LocalizedDescription = localizers[i].Description,
                    LocalizedName = localizers[i].Name,
                    StartDateTimeUTC = contests[i].StartDateTimeUTC,
                    EndDateTimeUTC = contests[i].EndDateTimeUTC,
                    Image = contests[i].Image,
                    ParticipantsCount = await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id),
                    ApprovalStatus = contests[i].ApprovalStatus
                });
            }
            return Json(publishedContests);
        }

        [HttpGet("get-participating-contests/{culture}")]
        [AuthorizeByJwt]
        public async Task<IActionResult> GetParticipatingContests(string culture)
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var userContests = await _dbContext.ContestsParticipants.Where(cp => cp.ParticipantId == currentUser.Id).ToListAsync();
            var contests = await _dbContext.Contests.Where(c => c.ApprovalStatus == ApproveType.Accepted
                                                            && c.EndDateTimeUTC >= DateTime.UtcNow
                                                            && userContests.Any(uc => uc.ContestId == c.Id))
                                                    .OrderBy(c => c.StartDateTimeUTC)
                                                    .ToListAsync();
            var localizers = contests.ConvertAll(c => c.ContestLocalizers.FirstOrDefault(cl => cl.Culture == culture));
            var publishedContests = new List<PublishedContest>();
            for (int i = 0; i < contests.Count; i++)
            {
                publishedContests.Add(new PublishedContest
                {
                    Id = contests[i].Id,
                    Creator = contests[i].Creator?.ResponseStructure,
                    LocalizedDescription = localizers[i].Description,
                    LocalizedName = localizers[i].Name,
                    StartDateTimeUTC = contests[i].StartDateTimeUTC,
                    EndDateTimeUTC = contests[i].EndDateTimeUTC,
                    Image = contests[i].Image,
                    ParticipantsCount = await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contests[i].Id),
                    ApprovalStatus = contests[i].ApprovalStatus
                });
            }
            return Json(publishedContests);
        }
        
        [HttpGet("{id}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetPublishedContest(long id, string culture)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == id);
            if (contest != null)
            {
                var localizer = contest.ContestLocalizers.FirstOrDefault(pl => pl.Culture == culture);
                if (localizer == null)
                {
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Такой локализации под данный контест не существует" }
                    });
                }
                var publishedContest = new PublishedContest
                {
                    Id = contest.Id,
                    LocalizedName = localizer.Name,
                    LocalizedDescription = localizer.Description,
                    Image = contest.Image,
                    StartDateTimeUTC = contest.StartDateTimeUTC,
                    EndDateTimeUTC = contest.EndDateTimeUTC,
                    Creator = contest.Creator?.ResponseStructure,
                    ParticipantsCount = await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == contest.Id),
                    ApprovalStatus = contest.ApprovalStatus
                };
                return Json(publishedContest);
            }
            return Json(new
            {
                status = false,
                errors = new List<string> { "Контеста с таким идентификатором не существует" }
            });
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetConstructedContest(long id)
        {
            var contest = await _dbContext.Contests.FirstOrDefaultAsync(p => p.Id == id);
            if (contest != null)
            {
                var constructedContest = new ConstructedContest
                {
                    Id = contest.Id,
                    Localizers = contest.ContestLocalizers,
                    Image = contest.Image,
                    StartDateTimeUTC = contest.StartDateTimeUTC,
                    EndDateTimeUTC = contest.EndDateTimeUTC,
                    DurationInMinutes = contest.DurationInMinutes,
                    Creator = contest.Creator?.ResponseStructure,
                    ApprovalStatus = contest.ApprovalStatus,
                    Rules = contest.RulesSet,
                    ApprovingModerator = contest.ApprovingModerator?.ResponseStructure,
                    ModerationMessage = contest.ModerationMessage,
                    Problems = (await _dbContext.ContestsProblems.Where(cp => cp.ContestId == contest.Id)
                                                                    .ToListAsync())
                                                                    .ConvertAll(cp =>
                                                                    {
                                                                        return new ProblemEntry
                                                                        {
                                                                            Letter = cp.Letter,
                                                                            ProblemId = cp.ProblemId,
                                                                            ContestId = cp.ContestId
                                                                        };
                                                                    })
                };
                return Json(constructedContest);
            }
            return Json(new
            {
                status = false,
                errors = new List<string> { "Контеста с таким идентификатором не существует" }
            });
        }

        [HttpGet("get-user-created-contests/{id}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserCreatedContests(long id, string culture)
        {
            var contests = await _dbContext.Contests.Where(c => c.CreatorId == id).ToListAsync();
            var publishedContests = contests.ConvertAll(async c =>
            {
                var localizer = c.ContestLocalizers.FirstOrDefault(pl => pl.Culture == culture);
                var pc = new PublishedContest
                {
                    Id = c.Id,
                    LocalizedName = localizer?.Name,
                    LocalizedDescription = localizer?.Description,
                    StartDateTimeUTC = c.StartDateTimeUTC,
                    EndDateTimeUTC = c.EndDateTimeUTC,
                    DurationInMinutes = c.DurationInMinutes,
                    Creator = c.Creator?.ResponseStructure,
                    Image = c.Image,
                    ModerationMessage = c.ModerationMessage,
                    ParticipantsCount = await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == c.Id),
                    ApprovalStatus = c.ApprovalStatus
                };
                return pc;
            });
            return Json(publishedContests.Select(pc => pc.Result));
        }

        [HttpPost("add-contest")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddContest([FromForm] ContestForm contestForm)
        {
            if (ModelState.IsValid)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(contestForm.Image.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)contestForm.Image.Length);
                }

                Contest contest = new Contest
                {
                    CreatorId = contestForm.CreatorUserId,
                    StartDateTimeUTC = contestForm.StartDateTimeUTC,
                    DurationInMinutes = contestForm.DurationInMinutes,
                    AreVirtualContestsAvailable = contestForm.AreVirtualContestsAvailable,
                    Image = Convert.ToBase64String(imageData),
                    IsPublic = contestForm.IsPublic,
                    ContestLocalizers = new List<ContestLocalizer>(),
                    RulesSetId = contestForm.RulesSetId
                };
                /*
                var user = await HttpContext.GetCurrentUser(_userManager);
                if (user.IsLimitedInContests)
                {
                    if (await _dbContext.Contests.AnyAsync(c => c.CreatorId == user.Id))
                    {
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Превышено ограничение недоверенного пользователя по созданию контестов" }
                        });
                    }
                    contest.ApprovalStatus = ApproveType.NotModeratedYet;
                }
                else
                {
                    contest.ApprovalStatus = ApproveType.Accepted;
                }*/
                contest.ApprovalStatus = ApproveType.Accepted;
                await _dbContext.Contests.AddAsync(contest);
                await _dbContext.SaveChangesAsync();
                for (int i = 0; i < contestForm.Localizers.Count; i++)
                {
                    var localizer = new ContestLocalizer
                    {
                        Culture = contestForm.Localizers[i].Culture,
                        Description = contestForm.Localizers[i].Description,
                        Name = contestForm.Localizers[i].Name,
                        ContestId = contest.Id
                    };
                    contest.ContestLocalizers.Add(localizer);
                }
                for (int i = 0; i < contestForm.Problems.Count; i++)
                {
                    var contestProblems = new ContestProblem
                    {
                        ContestId = contest.Id,
                        ProblemId = contestForm.Problems[i].ProblemId,
                        Letter = contestForm.Problems[i].Letter
                    };
                    await _dbContext.ContestsProblems.AddAsync(contestProblems);
                }
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
        [HttpPut("edit-contest/{id}")]
        public async Task<IActionResult> EditContest([FromForm] ContestForm contestForm, long id)
        {
            if (contestForm.Id == null || id <= 0 || id != contestForm.Id)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Id в запросе не совпадает с Id в форме" }
                });
            }
            if (ModelState.IsValid)
            {
                Contest contest = await _dbContext.Contests.FirstOrDefaultAsync(c => c.Id == id);
                if (contest == null)
                {
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Попытка изменить несуществующий контест" }
                    });
                }
                else
                {
                    if (HttpContext.GetCurrentUser().GetAwaiter().GetResult().Id != contest.CreatorId)
                    {
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Попытка изменить не свой контест" }
                        });
                    }
                    byte[] imageData = null;
                    using (var binaryReader = new BinaryReader(contestForm.Image.OpenReadStream()))
                    {
                        imageData = binaryReader.ReadBytes((int)contestForm.Image.Length);
                    }
                    contest.CreatorId = contestForm.CreatorUserId;
                    contest.Image = Convert.ToBase64String(imageData);
                    contest.StartDateTimeUTC = contestForm.StartDateTimeUTC;
                    contest.DurationInMinutes = contestForm.DurationInMinutes;
                    contest.AreVirtualContestsAvailable = contestForm.AreVirtualContestsAvailable;
                    contest.IsPublic = contestForm.IsPublic;
                    contest.RulesSetId = contestForm.RulesSetId;
                    _dbContext.Contests.Update(contest);
                    for (int i = 0; i < contestForm.Localizers.Count; i++)
                    {
                        var localizer = new ContestLocalizer
                        {
                            Culture = contestForm.Localizers[i].Culture,
                            Description = contestForm.Localizers[i].Description,
                            Name = contestForm.Localizers[i].Name,
                            ContestId = contest.Id
                        };
                        var loadedLocalizer = await _dbContext.ContestsLocalizers.FirstOrDefaultAsync(pl => pl.Culture == localizer.Culture && pl.ContestId == id);
                        if (loadedLocalizer == null)
                        {
                            await _dbContext.ContestsLocalizers.AddAsync(localizer);
                        }
                        else
                        {
                            loadedLocalizer.Description = localizer.Description;
                            loadedLocalizer.Name = localizer.Name;
                            _dbContext.ContestsLocalizers.Update(loadedLocalizer);
                        }
                    }
                    for (int i = 0; i < contestForm.Problems.Count; i++)
                    {
                        var contestProblem = new ContestProblem
                        {
                            ContestId = contest.Id,
                            ProblemId = contestForm.Problems[i].ProblemId,
                            Letter = contestForm.Problems[i].Letter
                        };
                        var loadedContestProblem = await _dbContext.ContestsProblems.FirstOrDefaultAsync(cp => cp.ProblemId == contestProblem.ProblemId && cp.ContestId == contestProblem.ContestId);
                        if (loadedContestProblem == null)
                        {
                            await _dbContext.ContestsProblems.AddAsync(contestProblem);
                        }
                        else
                        {
                            loadedContestProblem.Letter = contestForm.Problems[i].Letter;
                            _dbContext.ContestsProblems.Update(loadedContestProblem);
                        }
                    }
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
        [HttpDelete("delete-contest/{id}")]
        public async Task<IActionResult> DeleteContest(long id)
        {
            Contest loadedContest = await _dbContext.Contests.FindAsync(id);
            if (loadedContest == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Попытка удалить несуществующий контест" }
                });
            }
            var currentUser = await HttpContext.GetCurrentUser();
            var moderatorRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RolesContainer.Moderator);
            if (currentUser.Id != loadedContest.CreatorId && !currentUser.Roles.Contains(moderatorRole))
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Попытка удалить не свой контест или без админских прав" }
                });
            }
            _dbContext.Contests.Remove(loadedContest);
            await _dbContext.SaveChangesAsync();
            return Json(new
            {
                status = true,
                message = ""
            });
        }

        /*[HttpGet("get-contest-requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetAllPostsRequests()
        {
            var posts = await _dbContext.Posts.ToListAsync();
            var requests = posts.ConvertAll(p =>
            {
                PostRequest pr = new PostRequest
                {
                    Id = p.Id,
                    Author = p.Author,
                    PromotedDateTimeUTC = p.PromotedDateTimeUTC,
                    ApprovalStatus = p.ApprovalStatus,
                    ApprovingModerator = p.ApprovingModerator,
                    Localizers = p.PostLocalizers,
                    ModerationMessage = p.ModerationMessage
                };
                return pr;
            });
            return Json(requests);
        }

        [HttpGet("get-post-requests/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetPostRequest(long id)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Такого поста не существует" }
                });
            }
            var request = new PostRequest
            {
                Id = post.Id,
                Author = post.Author,
                PromotedDateTimeUTC = post.PromotedDateTimeUTC,
                ApprovalStatus = post.ApprovalStatus,
                ApprovingModerator = post.ApprovingModerator,
                Localizers = post.PostLocalizers,
                ModerationMessage = post.ModerationMessage
            };
            return Json(Request);
        }

        [HttpPut("approve-post/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ApproveOrRejectPost([FromBody] PostRequestForm postRequestForm, long id)
        {
            if (postRequestForm.Id != id || id < 0)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Id в запросе не совпадает с Id в форме" }
                });
            }
            if (ModelState.IsValid)
            {
                var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
                if (post == null)
                {
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Попытка модерировать несуществующий пост" }
                    });
                }
                else
                {
                    post.ApprovalStatus = postRequestForm.ApprovalStatus;
                    post.ApprovingModeratorId = postRequestForm.ApprovingModeratorId;
                    post.ModerationMessage = postRequestForm.ModerationMessage;
                    _dbContext.Posts.Update(post);
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
        }*/
    }
}
