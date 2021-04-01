﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestSystem.Extensions;
using ContestSystem.Models;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystemDbStructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContestSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtSettingsService _jwtSettingsService;

        public UsersController(ILogger<UsersController> logger, MainDbContext dbContext,
            UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager,
            JwtSettingsService jwtSettingsService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtSettingsService = jwtSettingsService;
        }

        [HttpPost("get-all-users")]
        [AuthorizeByJwt(Roles = RolesContainer.Admin)]
        public async Task<IActionResult> GetAllUsers()
        {
            var user = await HttpContext.GetCurrentUser(this._userManager);
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
        public async Task<IActionResult> UpdateUser([FromBody] User userFromBody)
        {
            if (ModelState.IsValid)
            {
                if (userFromBody.Id != default)
                {
                    var user = await _dbContext.Users.Where(u => u.Id == userFromBody.Id).Include(u => u.Roles)
                        .FirstAsync();
                    var selectedRoleNames = userFromBody.Roles.Select(r => r.Id).ToList();
                    user.FirstName = userFromBody.FirstName;
                    user.Surname = userFromBody.Surname;
                    user.Patronymic = userFromBody.Patronymic;
                    user.DateOfBirth = userFromBody.DateOfBirth;
                    user.Email = userFromBody.Email;
                    user.PhoneNumber = userFromBody.PhoneNumber;

                    var currentRoles = user.Roles.Select(r => r.Id).ToList();
                    user.Roles.RemoveAll(r => !selectedRoleNames.Contains(r.Id));
                    var rolesToAssign = selectedRoleNames.Except(currentRoles)
                        .Select(roleId => _dbContext.Roles.FirstOrDefault(r => r.Id == roleId))
                        .Where(r => r != null)
                        .ToList();
                    if (rolesToAssign.Count > 0)
                    {
                        user.Roles.AddRange(
                            rolesToAssign
                        );
                    }

                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
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