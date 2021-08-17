using ContestSystem.Models.DbContexts;
using ContestSystem.Models.Dictionaries;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Services;
using ContestSystemDbStructure.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestSystem.Areas.Blog.Controllers
{
    [Area("Blog")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class PostsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<PostsController> _logger;
        private readonly FileStorageService _storage;
        private readonly LocalizerHelperService _localizerHelper;

        private readonly string _entityName = Constants.PostEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public PostsController(MainDbContext dbContext, ILogger<PostsController> logger, FileStorageService storage,
            LocalizerHelperService localizerHelper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _storage = storage;
            _localizerHelper = localizerHelper;

            _errorCodes = Constants.ErrorCodes[_entityName];
        }

        [HttpGet("{culture}")]
        public async Task<IActionResult> GetLocalizedPosts(string culture) //TODO: ulong? offset = null, ulong? count = null
        {
            var posts = await _dbContext.Posts.Where(p => p.ApprovalStatus == ApproveType.Accepted).ToListAsync();
            var localizers = posts.ConvertAll(p => _localizerHelper.GetAppropriateLocalizer(p.PostLocalizers, culture));
            var localizedPosts = new List<PostLocalizedModel>();
            for (int i = 0; i < posts.Count; i++)
            {
                var lp = PostLocalizedModel.GetFromModel(posts[i], localizers[i], _storage.GetImageInBase64(posts[i].ImagePath));
                localizedPosts.Add(lp);
            }

            return Json(localizedPosts);
        }

        [HttpGet("user/{userId}/{culture}")]
        public async Task<IActionResult> GetUserLocalizedPosts(long userId, string culture)
        {
            var posts = await _dbContext.Posts.Where(p => p.AuthorId == userId).ToListAsync();
            List<PostBaseInfo> localizedPosts = posts.ConvertAll(p =>
            {
                var localizer = _localizerHelper.GetAppropriateLocalizer(p.PostLocalizers, culture);
                var lp = PostBaseInfo.GetFromModel(p, localizer, _storage.GetImageInBase64(p.ImagePath));
                return lp;
            });
            return Json(localizedPosts);
        }

        [HttpGet("{postId}/{culture}")]
        public async Task<IActionResult> GetLocalizedPost(long postId, string culture)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == postId);
            if (post != null)
            {
                var localizer = _localizerHelper.GetAppropriateLocalizer(post.PostLocalizers, culture);
                if (localizer == null)
                {
                    return NotFound(_errorCodes[Constants.EntityLocalizerDoesntExistErrorName]);
                }

                var localizedPost = PostLocalizedModel.GetFromModel(post, localizer, _storage.GetImageInBase64(post.ImagePath));
                return Json(localizedPost);
            }

            return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
        }
    }
}
