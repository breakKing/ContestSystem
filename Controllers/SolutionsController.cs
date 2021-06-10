using ContestSystemDbStructure.Models;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestSystem.Models.ExternalModels;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SolutionsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly CheckerSystemService _checkerSystemService;

        public SolutionsController(MainDbContext dbContext, UserManager<User> userManager, CheckerSystemService checkerSystemService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _checkerSystemService = checkerSystemService;
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetConstructedSolution(long id)
        {
            var solution = await _dbContext.Solutions.FirstOrDefaultAsync(s => s.Id == id);
            if (solution == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Нет решения с таким идентификатором" }
                });
            }
            var problemsInContest = await _dbContext.ContestsProblems.Where(cp => cp.ContestId == solution.ContestId).ToListAsync();
            var constructedSolution = ConstructedSolution.GetFromModel(solution, problemsInContest);
            return Json(constructedSolution);
        }
    }
}
