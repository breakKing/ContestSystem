using ContestSystem.Extensions;
using ContestSystem.Models;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.FormModels;
using ContestSystem.Models.Misc;
using ContestSystemDbStructure.Models.Auth;
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
    public class SessionController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<SessionController> _logger;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtSettingsService _jwtSettingsService;

        private readonly string _entityName = Constants.UserEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public SessionController(MainDbContext dbContext, ILogger<SessionController> logger, UserManager<User> userManager,
            RoleManager<Role> roleManager, SignInManager<User> signInManager, JwtSettingsService jwtSettingsService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtSettingsService = jwtSettingsService;

            _errorCodes = Constants.ErrorCodes[_entityName];
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginForm form)
        {
            User user = await _userManager.FindByNameAsync(form.Username);
            if (user is not null &&
                (await _signInManager.CheckPasswordSignInAsync(user, form.Password, false)).Succeeded)
            {
                var sessions = await _userManager.GetUserSessionsAsync(_dbContext, user.Id);

                var session = sessions.FirstOrDefault(s => s.Fingerprint == form.Fingerprint);

                if (session != null)
                {
                    await _userManager.RemoveUserSessionAsync(_dbContext, user.Id, session.RefreshToken.ToString());
                }

                if (sessions.Count >= Constants.MaxUserSessionsCount)
                {
                    await _userManager.DropAllUserSessionsAsync(_dbContext, user.Id);
                }

                int refreshTokenDuration = form.Remember ? Constants.LongTermRefreshTokenLifeTimeInHours : Constants.ShortTermRefreshTokenLifeTimeInHours;

                string refreshToken = await _userManager.CreateUserSessionAsync(_dbContext, user.Id, refreshTokenDuration, form.Fingerprint);

                if (refreshToken == null)
                {
                    _logger.LogWarning($"Попытка входа под пользователем с идентификатором {user.Id} с устройства с отпечатком " +
                        $"\"{form.Fingerprint}\" закончилась ошибкой генерации refresh-токена");

                    return Json(ResponseObject<long>.Fail(_errorCodes[Constants.AuthFailedErrorName]));
                }

                HttpContext.SetRefreshTokenCookie(refreshToken, refreshTokenDuration);

                _logger.LogInformation($"Пользователем с идентификатором {user.Id} был выполнен успешный вход в систему");
                return Json(new
                {
                    status = true,
                    user = user.ResponseStructure,
                    roles = await _userManager.GetRolesAsync(user),
                    token = _jwtSettingsService.GenerateTokenString(user, _userManager),
                    refresh = refreshToken
                });
            }
            return Json(ResponseObject<long>.Fail(_errorCodes[Constants.AuthFailedErrorName]));
        }

        [HttpPost("logout")]
        [AuthorizeByJwt]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenForm form)
        {
            var user = await HttpContext.GetCurrentUser(_userManager);

            var session = await _userManager.GetSessionByUserAndFingerprintAsync(_dbContext, user.Id, form.Fingerprint);

            if (session != null)
            {
                await _userManager.RemoveUserSessionAsync(_dbContext, user.Id, session.RefreshToken.ToString());
            }

            return Ok();
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
                            message = _errorCodes[Constants.EntityAlreadyExistsErrorName]
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
                    IsLimitedInContests = true,
                    IsLimitedInCourses = true,
                    IsLimitedInPosts = true,
                    IsLimitedInProblems = true,
                    Culture = string.IsNullOrWhiteSpace(userModel.Culture) ? "ru" : userModel.Culture
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
                            message = _errorCodes[Constants.TokenGenerationFailedErrorName],
                        });
                    }

                    string refreshToken = await _userManager.CreateUserSessionAsync(_dbContext, user.Id, Constants.ShortTermRefreshTokenLifeTimeInHours, userModel.Fingerprint);

                    HttpContext.SetRefreshTokenCookie(refreshToken, Constants.ShortTermRefreshTokenLifeTimeInHours);

                    return Json(new
                    {
                        status = true,
                        token,
                        refresh = refreshToken,
                        user = user?.ResponseStructure,
                        roles = await _userManager.GetRolesAsync(user),
                    });
                }
            }

            return Json(
                new
                {
                    status = false,
                    message = _errorCodes[Constants.UserRegisterFailedErrorName]
                });
        }

        [HttpPost("verify-token")]
        public async Task<IActionResult> VerifyToken([FromBody] RefreshTokenForm form)
        {
            bool canRefresh = HttpContext.Request.Cookies.TryGetValue(Constants.RefreshTokenCookieName, out string refreshToken)
                                || !string.IsNullOrWhiteSpace(form.RefreshToken);

            if (canRefresh)
            {
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    refreshToken = form.RefreshToken;
                }

                var session = await _userManager.GetSessionByRefreshTokenAndFingerprintAsync(_dbContext, refreshToken, form.Fingerprint);

                if (session != null)
                {
                    var user = session.User;

                    var newRefreshToken = await _userManager.UpdateRefreshTokenAsync(_dbContext, user.Id, refreshToken);

                    if (newRefreshToken != null)
                    {
                        HttpContext.SetRefreshTokenCookie(newRefreshToken, session.ExpiresInHours);

                        return Json(
                           new
                           {
                               user = user?.ResponseStructure,
                               roles = await _userManager.GetRolesAsync(user),
                               token = _jwtSettingsService.GenerateTokenString(user, _userManager),
                               refresh = refreshToken
                           });
                    }
                }
            }

            return Json(ResponseObject<long>.Fail(_errorCodes[Constants.VerifyTokenFailedErrorName]));
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
