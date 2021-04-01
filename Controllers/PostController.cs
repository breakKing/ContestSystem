using ContestSystem.Models.Input;
using ContestSystem.Models.Output;
using ContestSystemDbStructure;
using ContestSystemDbStructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContestSystem.Models.DbContexts;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly MainDbContext _dbContext;

        public PostController(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/<PostController>
        [HttpGet("{count}/{page}")]
        public async Task<ActionResult<IEnumerable<PostOutputModel>>> GetPostsPage(int count, int page)
        {
            if (count <= 0 || page <= 0)
            {
                return BadRequest();
            }

            List<Post> loadedPosts = await _dbContext.Posts.OrderByDescending(post => post.PublicationDateTimeUTC)
                                                                    .Skip((page - 1) * count)
                                                                    .Take(count)
                                                                    .ToListAsync();

            List<PostOutputModel> postOutputs = (List<PostOutputModel>)loadedPosts.ConvertAll(async post =>
                                                                                    {
                                                                                        PostOutputModel postOut = new PostOutputModel();
                                                                                        await postOut.TransformForOutputAsync(post);
                                                                                        return postOut;
                                                                                    })
                                                                                    .Select(postOut => postOut.Result);

            return postOutputs;
        }

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostOutputModel>> GetPost(long id)
        {
            Post loadedPost = await _dbContext.Posts.FindAsync(id);
            if (loadedPost == null)
            {
                return NotFound();
            }
            PostOutputModel postOutput = new PostOutputModel();
            await postOutput.TransformForOutputAsync(loadedPost);
            return postOutput;
        }

        // POST api/<PostController>
        [HttpPost]
        public async Task<ActionResult<PostOutputModel>> AddPost([FromBody] PostInputModel postInput)
        {
            PostBaseModel post = await postInput.ReadFromInputAsync();
            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();
            PostOutputModel postOutput = new PostOutputModel();
            await postOutput.TransformForOutputAsync(post);
            return CreatedAtAction("GetPost", new { id = post.Id }, postOutput);
        }

        // PUT api/<PostController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult<PostOutputModel>> EditPost(long id, [FromBody] PostInputModel postInput)
        {
            if (id <= 0 || id != postInput.Id)
            {
                return BadRequest();
            }
            Post post = await postInput.ReadFromInputAsync();
            _dbContext.Entry(post).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_dbContext.Posts.Any(post => post.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    return Conflict();
                }
            }
            return NoContent();
        }

        // DELETE api/<PostController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            Post loadedPost = await _dbContext.Posts.FindAsync(id);
            if (loadedPost == null)
            {
                return NotFound();
            }
            _dbContext.Posts.Remove(loadedPost);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
