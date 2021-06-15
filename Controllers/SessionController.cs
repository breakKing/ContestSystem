using System.Threading.Tasks;
using ContestSystemDbStructure.Models;
using ContestSystem.Extensions;
using ContestSystem.Models;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.FormModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ContestSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : Controller
    {
        private readonly ILogger<SessionController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtSettingsService _jwtSettingsService;

        public SessionController(ILogger<SessionController> logger, UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager, JwtSettingsService jwtSettingsService)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtSettingsService = jwtSettingsService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginForm form)
        {
            User user = await _userManager.FindByNameAsync(form.username);
            if (user is not null &&
                (await _signInManager.CheckPasswordSignInAsync(user, form.password, false)).Succeeded)
            {
                _logger.LogInformation($"Пользователем с идентификатором {user.Id} был выполнен успешный вход в систему");
                return Json(new
                {
                    status = true,
                    user = user.ResponseStructure,
                    roles = await _userManager.GetRolesAsync(user),
                    token = _jwtSettingsService.GenerateTokenString(user, _userManager)
                });
            }
            return Json(new
            {
                status = false,
                message = "Не верный логин или пароль",
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationModel userModel)
        {
            if (ModelState.IsValid)
            {
                var existsUser = await _userManager.FindByNameAsync(userModel.UserName);
                if (existsUser is not null)
                {
                    return Json(
                        new
                        {
                            status = false,
                            message = "Такой пользователь уже зарегистрирован"
                        });
                }

                var user = new User()
                {
                    UserName = userModel.UserName,
                    Email = userModel.Email,
                    FirstName = userModel.FirstName,
                    Surname = userModel.Surname,
                    Patronymic = userModel.Patronymic,
                    PhoneNumber = userModel.Phone,
                    DateOfBirth = userModel.DateOfBirth,
                };
                var result = await _userManager.CreateAsync(user, userModel.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Успешно создан и зарегистрирован пользователь с идентификатором {user.Id}");
                    // default role
                    await _userManager.AddToRoleAsync(user, RolesContainer.User);

                    var token = _jwtSettingsService.GenerateTokenString(user, _userManager);
                    if (token is null)
                    {
                        return Json(new
                        {
                            status = false,
                            message = "Не удалось создать Jwt токен",
                        });
                    }

                    return Json(new
                    {
                        status = true,
                        token,
                        user = user?.ResponseStructure,
                        roles = await _userManager.GetRolesAsync(user),
                    });
                }
            }

            return Json(
                new
                {
                    status = false,
                    message = "Не удалось зарегистрировать пользователя"
                });
        }

        [HttpPost("verify-token")]
        [AuthorizeByJwt]
        public async Task<IActionResult> VerifyToken()
        {
            var user = await HttpContext.GetCurrentUser(_userManager);
            return Json(
                new
                {
                    user = user?.ResponseStructure,
                    roles = await _userManager.GetRolesAsync(user),
                    token = _jwtSettingsService.GenerateTokenString(user, _userManager)
                });
        }

        [AuthorizeByJwt]
        [HttpPost("get-all-roles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return Json(new
            {
                roles
            });
        }
    }
}