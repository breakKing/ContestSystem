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

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : Controller
    {
        private readonly MainDbContext _dbContext;

        public PostsController(MainDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{culture}")]
        public async Task<IActionResult> GetAllPublishedPosts(string culture)
        {
            var posts = await _dbContext.Posts.Where(p => p.ApprovalStatus == ApproveType.Accepted).ToListAsync();
            var localizers = posts.ConvertAll(post => post.PostLocalizers.FirstOrDefault(pl => pl.Culture == culture));
            var publishedPosts = new List<PublishedPost>();
            for (int i = 0; i < posts.Count; i++)
            {
                var pp = PublishedPost.GetFromModel(posts[i], localizers[i]);
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
                var pp = PublishedPost.GetFromModel(p, localizer);
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
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Такой локализации под данный пост не существует" }
                    });
                }

                var publishedPost = PublishedPost.GetFromModel(post, localizer);
                return Json(publishedPost);
            }

            return Json(new
            {
                status = false,
                errors = new List<string> { "Поста с таким идентификатором не существует" }
            });
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetConstructedPost(long id)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post != null)
            {
                var constructedPost = ConstructedPost.GetFromModel(post);
                return Json(constructedPost);
            }
            return Json(new
            {
                status = false,
                errors = new List<string> { "Поста с таким идентификатором не существует" }
            });
        }

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpPost("add-post")]
        public async Task<IActionResult> AddPost([FromForm] PostForm postForm)
        {
            if (ModelState.IsValid)
            {
                byte[] imageData = null;
                if (postForm.PreviewImage != null)
                {
                    await using (var ms = new MemoryStream())
                    {
                        await postForm.PreviewImage.CopyToAsync(ms);
                        imageData = ms.ToArray();
                    }
                }

                Post post = new Post
                {
                    PromotedDateTimeUTC = DateTime.UtcNow,
                    AuthorId = postForm.AuthorUserId,
                    PreviewImage = imageData == null ? null : Convert.ToBase64String(imageData),
                    PostLocalizers = new List<PostLocalizer>()
                };
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == postForm.AuthorUserId);
                if (user == null)
                {
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> { "Автор является несуществующим пользователем" }
                    });
                }
                if (user.IsLimitedInPosts)
                {
                    if (await _dbContext.Posts.CountAsync(p => p.AuthorId == user.Id && p.ApprovalStatus == ApproveType.NotModeratedYet) == 1)
                    {
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
                return Json(new
                {
                    status = true,
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
            if (postForm.Id == null || id <= 0 || id != postForm.Id)
            {
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
                    return Json(new
                    {
                        status = false,
                        errors = new List<string> {"Попытка изменить несуществующий пост"}
                    });
                }
                else
                {
                    if (HttpContext.GetCurrentUser().GetAwaiter().GetResult().Id != post.AuthorId)
                    {
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> {"Попытка изменить не свой пост"}
                        });
                    }

                    byte[] imageData = null;
                    if (postForm.PreviewImage != null)
                    {
                        await using (var ms = new MemoryStream())
                        {
                            await postForm.PreviewImage.CopyToAsync(ms);
                            imageData = ms.ToArray();
                        }
                    }

                    post.AuthorId = postForm.AuthorUserId;
                    if (imageData != null)
                    {
                        post.PreviewImage = Convert.ToBase64String(imageData);
                    }

                    if (post.ApprovalStatus == ApproveType.Rejected)
                    {
                        post.ApprovalStatus = ApproveType.NotModeratedYet;
                    }
                    else if (post.ApprovalStatus == ApproveType.Accepted)
                    {
                        post.PublicationDateTimeUTC = DateTime.UtcNow;
                    }

                    _dbContext.Posts.Update(post);
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
                        var loadedLocalizer =
                            await _dbContext.PostsLocalizers.FirstOrDefaultAsync(pl =>
                                pl.Culture == localizer.Culture && pl.PostId == id);
                        if (loadedLocalizer == null)
                        {
                            await _dbContext.PostsLocalizers.AddAsync(localizer);
                        }
                        else
                        {
                            loadedLocalizer.PreviewText = localizer.PreviewText;
                            loadedLocalizer.Name = localizer.Name;
                            loadedLocalizer.HtmlText = localizer.HtmlText;
                            _dbContext.PostsLocalizers.Update(loadedLocalizer);
                        }
                    }

                    try
                    {
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> {"Ошибка параллельного сохранения"}
                        });
                    }

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
            Post loadedPost = await _dbContext.Posts.FindAsync(id);
            if (loadedPost == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить несуществующий пост"}
                });
            }

            var currentUser = await HttpContext.GetCurrentUser();
            var moderatorRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RolesContainer.Moderator);
            if (currentUser.Id != loadedPost.AuthorId && !currentUser.Roles.Contains(moderatorRole))
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> {"Попытка удалить не свой пост или без админских прав"}
                });
            }

            _dbContext.Posts.Remove(loadedPost);
            await _dbContext.SaveChangesAsync();
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
                ConstructedPost pr = ConstructedPost.GetFromModel(p);
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
                ConstructedPost pr = ConstructedPost.GetFromModel(p);
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
                ConstructedPost pr = ConstructedPost.GetFromModel(p);
                return pr;
            });
            return Json(requests);
        }

        [HttpPut("moderate/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ApproveOrRejectPost([FromBody] PostRequestForm postRequestForm, long id)
        {
            if (postRequestForm.PostId != id || id < 0)
            {
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
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> {"Ошибка параллельного сохранения"}
                        });
                    }

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