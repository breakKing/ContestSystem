using ContestSystemDbStructure.Models;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
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
    public class CoursesController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public CoursesController(MainDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("get-user-created-courses/{id}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserCreatedContests(long id, string culture)
        {
            /*var contests = await _dbContext.Contests.Where(c => c.CreatorId == id).ToListAsync();
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
                    ParticipantsCount = await _dbContext.ContestsParticipants.CountAsync(cp => cp.ContestId == c.Id)
                };
                return pc;
            });
            return Json(publishedContests.Select(pc => pc.Result));*/
            return null;
        }
    }
}
