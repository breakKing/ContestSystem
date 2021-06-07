using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using ContestSystem.Services;
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
    public class CheckersController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly CheckerSystemService _checkerSystemService;

        public CheckersController(MainDbContext dbContext, UserManager<User> userManager,
            CheckerSystemService checkerSystemService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _checkerSystemService = checkerSystemService;
        }

        [HttpGet("get-user-checkers/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserCheckers(long id)
        {
            var checkers = await _dbContext.Checkers.Where(p => p.AuthorId == id).ToListAsync();
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
            var checkers = await _dbContext.Checkers.Where(c => c.AuthorId == id || c.IsPublic).ToListAsync();
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
            var checker = await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id);
            if (checker == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Такого чекера не существует"}
                });
            }

            var publishedChecker = PublishedChecker.GetFromModel(checker);
            return Json(publishedChecker);
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetConstructedChecker(long id)
        {
            var checker = await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id);
            if (checker == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Такого чекера не существует"}
                });
            }

            var constructedChecker = ConstructedChecker.GetFromModel(checker);
            return Json(constructedChecker);
        }

        [HttpPost("add-checker")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> AddChecker([FromBody] CheckerForm checkerForm)
        {
            if (ModelState.IsValid)
            {
                var checker = new Checker
                {
                    AuthorId = checkerForm.AuthorId,
                    Code = checkerForm.Code,
                    Name = checkerForm.Name,
                    Description = checkerForm.Description,
                    IsPublic = checkerForm.IsPublic
                };
                //TODO Сделать норм проверку на лимиты вместо автоматического компила
                checker.ApprovalStatus = ApproveType.Accepted;
                _dbContext.Checkers.Add(checker);
                await _dbContext.SaveChangesAsync();
                if (checker.ApprovalStatus == ApproveType.Accepted)
                {
                    checker = await _checkerSystemService.SendCheckerForCompilationAsync(checker);
                    _dbContext.Checkers.Update(checker);
                    await _dbContext.SaveChangesAsync();
                }
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

        [HttpPost("edit-checker/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> EditChecker([FromBody] CheckerForm checkerForm, long id)
        {
            if (checkerForm.Id == null || checkerForm.Id.Value != id)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Id в форме не совпадает с Id в запросе"}
                });
            }

            if (ModelState.IsValid)
            {
                var checker = await _dbContext.Checkers.FirstOrDefaultAsync(ch => ch.Id == id);
                checker.AuthorId = checkerForm.AuthorId;
                checker.Code = checkerForm.Code;
                checker.Name = checkerForm.Name;
                checker.Description = checkerForm.Description;
                checker.IsPublic = checkerForm.IsPublic;
                //TODO Сделать норм проверку на лимиты вместо автоматического компила
                checker.ApprovalStatus = ApproveType.Accepted;
                if (checker.ApprovalStatus == ApproveType.Accepted)
                {
                    checker = await _checkerSystemService.SendCheckerForCompilationAsync(checker);
                    _dbContext.Checkers.Update(checker);
                    await _dbContext.SaveChangesAsync();
                }

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
        [HttpDelete("delete-checker/{id}")]
        public async Task<IActionResult> DeleteChecker(long id)
        {
            Checker loadedChecker = await _dbContext.Checkers.FindAsync(id);
            if (loadedChecker == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить несуществующий чекер"}
                });
            }

            var currentUser = await HttpContext.GetCurrentUser();
            var moderatorRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RolesContainer.Moderator);
            if (currentUser.Id != loadedChecker.AuthorId && !currentUser.Roles.Contains(moderatorRole))
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить не свой пост или без модераторских прав"}
                });
            }

            _dbContext.Checkers.Remove(loadedChecker);
            await _dbContext.SaveChangesAsync();
            return Json(new
            {
                status = true,
                message = ""
            });
        }
    }
}