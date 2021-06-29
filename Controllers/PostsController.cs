using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ContestSystem.Services;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<PostsController> _logger;
        private readonly FileStorageService _storage;

        public PostsController(MainDbContext dbContext, ILogger<PostsController> logger, FileStorageService storage)
        {
            _dbContext = dbContext;
            _logger = logger;
            _storage = storage;
        }

        [HttpGet("{culture}")]
        public async Task<IActionResult> GetAllPublishedPosts(string culture)
        {
            var posts = await _dbContext.Posts.Where(p => p.ApprovalStatus == ApproveType.Accepted).ToListAsync();
            var localizers = posts.ConvertAll(post => post.PostLocalizers.FirstOrDefault(pl => pl.Culture == culture));
            var publishedPosts = new List<PublishedPost>();
            for (int i = 0; i < posts.Count; i++)
            {
                var pp = PublishedPost.GetFromModel(posts[i], localizers[i], _storage.GetPostImageInBase64(posts[i].Id));
                publishedPosts.Add(pp);
            }

            return Json(publishedPosts);
        }

        [HttpGet("get-user-posts/{id}/{culture}")]
        [AuthorizeByJwt(Roles = RolesContainer.User)]
        public async Task<IActionResult> GetUserPosts(long id, string culture)
        {
            var posts = await _dbContext.Posts.Where(p => p.AuthorId == id).ToListAsync();
            List<PublishedPost> publishedPosts = posts.ConvertAll(p =>
            {
                var localizer = p.PostLocalizers.FirstOrDefault(pl => pl.Culture == culture);
                var pp = PublishedPost.GetFromModel(p, localizer, _storage.GetPostImageInBase64(p.Id));
                return pp;
            });
            return Json(publishedPosts);
        }

        [HttpGet("{id}/{culture}")]
        public async Task<IActionResult> GetPublishedPost(long id, string culture)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post != null)
            {
                var localizer = post.PostLocalizers.FirstOrDefault(pl => pl.Culture == culture);
                if (localizer == null)
                {
                    return NotFound("Такой локализации под пост не существует");
                }

                var publishedPost = PublishedPost.GetFromModel(post, localizer, _storage.GetPostImageInBase64(post.Id));
                return Json(publishedPost);
            }

            return NotFound("Такого поста не существует");
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetConstructedPost(long id)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post != null)
            {
                var constructedPost = ConstructedPost.GetFromModel(post, _storage.GetPostImageInBase64(post.Id));
                return Json(constructedPost);
            }
            return NotFound("Такого поста не существует");
        }

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpPost("add-post")]
        public async Task<IActionResult> AddPost([FromForm] PostForm postForm)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await HttpContext.GetCurrentUser();

                Post post = new Post
                {
                    PromotedDateTimeUTC = DateTime.UtcNow,
                    AuthorId = postForm.AuthorUserId,
                    PostLocalizers = new List<PostLocalizer>()
                };
                if (currentUser.Id != postForm.AuthorUserId)
                {
                    _logger.LogCreationByNonEqualCurrentUserAndCreator("Post", currentUser.Id, postForm.AuthorUserId);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Id автора в форме отличается от Id текущего пользователя" }
                    });
                }
                if (currentUser.IsLimitedInPosts)
                {
                    if (await _dbContext.Posts.CountAsync(p => p.AuthorId == currentUser.Id && p.ApprovalStatus == ApproveType.NotModeratedYet) == 1)
                    {
                        _logger.LogCreationFailedBecauseOfLimits("Post", currentUser.Id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Превышено ограничение недоверенного пользователя по созданию постов" }
                        });
                    }
                    post.ApprovalStatus = ApproveType.NotModeratedYet;
                }
                else
                {
                    post.ApprovalStatus = ApproveType.Accepted;
                }
                _dbContext.Posts.Add(post);
                await _dbContext.SaveChangesAsync();
                await _storage.SavePostImageAsync(post.Id, postForm.PreviewImage);
                for (int i = 0; i < postForm.Localizers.Count; i++)
                {
                    var localizer = new PostLocalizer
                    {
                        Culture = postForm.Localizers[i].Culture,
                        PreviewText = postForm.Localizers[i].PreviewText,
                        Name = postForm.Localizers[i].Name,
                        HtmlText = postForm.Localizers[i].HtmlText,
                        PostId = post.Id
                    };
                    post.PostLocalizers.Add(localizer);
                }
                await _dbContext.SaveChangesAsync();
                if (post.ApprovalStatus == ApproveType.Accepted)
                {
                    _logger.LogCreationSuccessfulWithAutoAccept("Post", post.Id, currentUser.Id);
                }
                else
                {
                    _logger.LogCreationSuccessful("Post", post.Id, currentUser.Id);
                }
                return Json(new
                {
                    status = true,
                    data = post.Id,
                    errors = new List<string>()
                });
            }

            return Json(new
            {
                status = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpPut("edit-post/{id}")]
        public async Task<IActionResult> EditPost([FromForm] PostForm postForm, long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();

            if (postForm.Id == null || id <= 0 || id != postForm.Id)
            {
                _logger.LogEditingWithNonEqualFormAndRequestId("Post", postForm.Id, id, currentUser.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Id в запросе не совпадает с Id в форме"}
                });
            }

            if (ModelState.IsValid)
            {
                Post post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
                if (post == null)
                {
                    _logger.LogEditingOfNonExistentEntity("Post", id, currentUser.Id);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Попытка изменить несуществующий пост"}
                    });
                }
                else
                {
                    if (currentUser.Id != post.AuthorId)
                    {
                        _logger.LogEditingByNotAppropriateUser("Post", id, currentUser.Id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> {"Попытка изменить не свой пост"}
                        });
                    }

                    await _storage.SavePostImageAsync(id, postForm.PreviewImage);

                    if (post.ApprovalStatus == ApproveType.Rejected)
                    {
                        post.ApprovalStatus = ApproveType.NotModeratedYet;
                        post.ApprovingModeratorId = null;
                    }
                    else if (post.ApprovalStatus == ApproveType.Accepted)
                    {
                        post.PublicationDateTimeUTC = DateTime.UtcNow;
                    }
                    _dbContext.Posts.Update(post);

                    var localizers = await _dbContext.PostsLocalizers.Where(l => l.PostId == id).ToListAsync();
                    var localizersExamined = new Dictionary<long, bool>();
                    foreach (var l in localizers)
                    {
                        localizersExamined.Add(l.Id, false);
                    }
                    for (int i = 0; i < postForm.Localizers.Count; i++)
                    {
                        var localizer = new PostLocalizer
                        {
                            Culture = postForm.Localizers[i].Culture,
                            PreviewText = postForm.Localizers[i].PreviewText,
                            Name = postForm.Localizers[i].Name,
                            HtmlText = postForm.Localizers[i].HtmlText,
                            PostId = post.Id
                        };
                        var loadedLocalizer = localizers.FirstOrDefault(pl => pl.Culture == localizer.Culture);
                        if (loadedLocalizer == null)
                        {
                            await _dbContext.PostsLocalizers.AddAsync(localizer);
                        }
                        else
                        {
                            localizersExamined[loadedLocalizer.Id] = true;
                            loadedLocalizer.PreviewText = localizer.PreviewText;
                            loadedLocalizer.Name = localizer.Name;
                            loadedLocalizer.HtmlText = localizer.HtmlText;
                            _dbContext.PostsLocalizers.Update(loadedLocalizer);
                        }
                    }
                    foreach (var item in localizersExamined)
                    {
                        if (!item.Value)
                        {
                            var loadedLocalizer = localizers.FirstOrDefault(l => l.Id == item.Key);
                            _dbContext.PostsLocalizers.Remove(loadedLocalizer);
                        }
                    }

                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _logger.LogParallelSaveError("Post", id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> {"Ошибка параллельного сохранения"}
                        });
                    }
                    _logger.LogEditingSuccessful("Post", id, currentUser.Id);
                    return Json(new
                    {
                        status = true,
                        errors = new List<string>()
                    });
                }
            }

            return Json(new
            {
                status = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }

        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        [HttpDelete("delete-post/{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();
            Post loadedPost = await _dbContext.Posts.FindAsync(id);
            if (loadedPost == null)
            {
                _logger.LogDeletingOfNonExistentEnitiy("Post", id, currentUser.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить несуществующий пост"}
                });
            }
            var moderatorRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RolesContainer.Moderator);
            if (currentUser.Id != loadedPost.AuthorId && !currentUser.Roles.Contains(moderatorRole))
            {
                _logger.LogDeletingByNotAppropriateUser("Post", id, currentUser.Id);
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить не свой пост или без админских прав"}
                });
            }

            _dbContext.Posts.Remove(loadedPost);
            await _dbContext.SaveChangesAsync();
            _logger.LogDeletingSuccessful("Post", id, currentUser.Id);
            return Json(new
            {
                status = true,
                errors = new List<string>()
            });
        }

        [HttpGet("get-requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetPostsRequests()
        {
            var posts = await _dbContext.Posts.Where(p => p.ApprovalStatus == ApproveType.NotModeratedYet).ToListAsync();
            var requests = posts.ConvertAll(p =>
            {
                ConstructedPost pr = ConstructedPost.GetFromModel(p, _storage.GetPostImageInBase64(p.Id));
                return pr;
            });
            return Json(requests);
        }

        [HttpGet("get-approved")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetApprovedPosts()
        {
            var posts = await _dbContext.Posts.Where(p => p.ApprovalStatus == ApproveType.Accepted).ToListAsync();
            var requests = posts.ConvertAll(p =>
            {
                ConstructedPost pr = ConstructedPost.GetFromModel(p, _storage.GetPostImageInBase64(p.Id));
                return pr;
            });
            return Json(requests);
        }

        [HttpGet("get-rejected")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetRejectedPosts()
        {
            var posts = await _dbContext.Posts.Where(p => p.ApprovalStatus == ApproveType.Rejected).ToListAsync();
            var requests = posts.ConvertAll(p =>
            {
                ConstructedPost pr = ConstructedPost.GetFromModel(p, _storage.GetPostImageInBase64(p.Id));
                return pr;
            });
            return Json(requests);
        }

        [HttpPut("moderate/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ApproveOrRejectPost([FromBody] PostRequestForm postRequestForm, long id)
        {
            var currentUser = await HttpContext.GetCurrentUser();

            if (postRequestForm.PostId != id || id < 0)
            {
                _logger.LogModeratingWithNonEqualFormAndRequestId("Post", postRequestForm.PostId, id, currentUser.Id);
                return Json(new
                {
                    success = false,
                    errors = new List<string> {"Id в запросе не совпадает с Id в форме"}
                });
            }

            if (ModelState.IsValid)
            {
                var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
                if (post == null)
                {
                    _logger.LogModeratingOfNonExistentEntity("Post", id, currentUser.Id);
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Попытка модерировать несуществующий пост"}
                    });
                }
                else
                {
                    post.ApprovalStatus = postRequestForm.ApprovalStatus;
                    post.ApprovingModeratorId = postRequestForm.ApprovingModeratorId;
                    post.ModerationMessage = postRequestForm.ModerationMessage;
                    post.PublicationDateTimeUTC = DateTime.UtcNow;
                    _dbContext.Posts.Update(post);
                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        _logger.LogParallelSaveError("Post", id);
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> {"Ошибка параллельного сохранения"}
                        });
                    }
                    _logger.LogModeratingSuccessful("Post", id, currentUser.Id, postRequestForm.ApprovalStatus);
                    return Json(new
                    {
                        status = true,
                        errors = new List<string>()
                    });
                }
            }

            return Json(new
            {
                status = false,
                errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage).ToList()
            });
        }
    }
}