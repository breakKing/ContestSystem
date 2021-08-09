using ContestSystem.Models.Attributes;
using ContestSystemDbStructure.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ContestSystem.Areas.Contests.Controllers
{
    [Area("Contests")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class ManagementController : Controller
    {
        public ManagementController()
        {

        }

        [HttpGet("solutions/{contestId}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetAllSolutions(long contestId)
        {
            return null;
        }
    }
}
