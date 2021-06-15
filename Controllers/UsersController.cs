using System.Linq;
using System.Threading.Tasks;
using ContestSystemDbStructure.Models;
using ContestSystem.Extensions;
using ContestSystem.Models;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.FormModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ContestSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public UsersController(ILogger<UsersController> logger, MainDbContext dbContext, UserManager<User> userManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpPost("get-all-users")]
        [AuthorizeByJwt(Roles = RolesContainer.Admin)]
        public async Task<IActionResult> GetAllUsers()
        {
            var user = await HttpContext.GetCurrentUser(_userManager);
            var users = await _dbContext.Users
                .Where(u => (user == null || u.Id != user.Id))
                .Include(u => u.Roles)
                .ToListAsync();
            return Json(
                new
                {
                    users
                });
        }

        [HttpPost("update-user")]
        [AuthorizeByJwt(Roles = RolesContainer.Admin)]
        public async Task<IActionResult> UpdateUser([FromBody] UserSavingForm userFromBody)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            if (ModelState.IsValid)
            {
                if (userFromBody.Id != default)
                {
                    var user = await _dbContext.Users
                        .Where(u => u.Id == userFromBody.Id)
                        .Include(u => u.Roles)
                        .FirstAsync();

                    user.FirstName = userFromBody.FirstName;
                    user.Surname = userFromBody.Surname;
                    user.Patronymic = userFromBody.Patronymic;
                    user.DateOfBirth = userFromBody.DateOfBirth;
                    user.Email = userFromBody.Email;
                    user.PhoneNumber = userFromBody.PhoneNumber;
                    user.IsLimitedInContests = userFromBody.IsLimitedInContests;
                    user.IsLimitedInPosts = userFromBody.IsLimitedInPosts;
                    user.IsLimitedInCourses = userFromBody.IsLimitedInCourses;
                    user.IsLimitedInProblems = userFromBody.IsLimitedInProblems;
                    var rolesToAssign = userFromBody.Roles
                        .Select(roleName => _dbContext.Roles.FirstOrDefault(r => r.Name == roleName))
                        .Where(r => r != null)
                        .ToList();
                    user.Roles = rolesToAssign;
                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _logger.LogParallelSaveError("User", user.Id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Ошибка параллельного сохранения" }
                        });
                    }
                    _logger.LogEditingSuccessful("User", user.Id, currentUser.Id);
                    return Json(new
                    {
                        success = true
                    });
                }
            }

            return Json(new
            {
                success = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }
    }
}