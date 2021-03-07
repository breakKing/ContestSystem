﻿using ContestSystemDbStructure;
using ContestSystemDbStructure.BaseModels;
using Microsoft.AspNetCore.Http;
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
    public class TestDbController : ControllerBase
    {
        private ContestSystemDbContext _dbContext;

        public TestDbController(ContestSystemDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Route("contests")]
        public ActionResult<IEnumerable<ContestBaseModel>> GetContests()
        {
            return _dbContext.Contests.ToList();
        }

        [HttpGet]
        [Route("problems")]
        public ActionResult<IEnumerable<ProblemBaseModel>> GetProblems()
        {
            return _dbContext.Problems.ToList();
        }

        [HttpGet]
        [Route("examples")]
        public ActionResult<IEnumerable<ExampleBaseModel>> GetExamples()
        {
            return _dbContext.Examples.Include(ex => ex.Problem).ToList();
        }

        [HttpGet]
        [Route("contests_problems")]
        public ActionResult<IEnumerable<ContestsProblemsBaseModel>> GetAliases()
        {
            return _dbContext.ContestsProblems.Include(pa => pa.Contest).Include(pa => pa.Problem).ToList();
        }
    }
}
