using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.FormModels;
using ContestSystem.DbStructure.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Areas.Auth.Controllers
{
    [Area("Auth")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        private readonly string _entityName = Constants.UserEntityName;

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
                    user.Culture = string.IsNullOrWhiteSpace(userFromBody.Culture) ? "ru" : userFromBody.Culture;
                    var rolesToAssign = userFromBody.Roles
                        .Select(roleName => _dbContext.Roles.FirstOrDefault(r => r.Name == roleName))
                        .Where(r => r != null)
                        .ToList();
                    user.Roles = rolesToAssign;
                    if (!await _dbContext.SecureSaveAsync())
                    {
                        _logger.LogDbSaveError(_entityName, user.Id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { Constants.ErrorCodes[Constants.CommonSectionName][Constants.DbSaveErrorName] }
                        });
                    }
                    _logger.LogEditingSuccessful(_entityName, user.Id, currentUser.Id);
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
