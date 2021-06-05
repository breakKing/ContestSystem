using ContestSystemDbStructure.Enums;
using ContestSystemDbStructure.Models;
using ContestSystem.Extensions;
using ContestSystem.Models.Attributes;
using ContestSystem.Models.DbContexts;
using ContestSystem.Models.ExternalModels;
using ContestSystem.Models.FormModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public PostsController(MainDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("{culture}")]
        public async Task<IActionResult> GetAllPublishedPosts(string culture)
        {
            var posts = await _dbContext.Posts.Where(p => p.ApprovalStatus == ApproveType.Accepted).ToListAsync();
            var localizers = posts.ConvertAll(post => post.PostLocalizers.FirstOrDefault(pl => pl.Culture == culture));
            var publishedPosts = new List<PublishedPost>();
            for (int i = 0; i < posts.Count; i++)
            {
                publishedPosts.Add(new PublishedPost
                {
                    Id = posts[i].Id,
                    Author = posts[i].Author,
                    HtmlLocalizedText = null,
                    LocalizedName = localizers[i].Name,
                    PublicationDateTimeUTC = posts[i].PublicationDateTimeUTC,
                    PreviewImage = posts[i].PreviewImage,
                    PreviewText = localizers[i].PreviewText,
                    ApprovalStatus = posts[i].ApprovalStatus
                });
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
                PublishedPost pp = new PublishedPost
                {
                    Id = p.Id,
                    LocalizedName = localizer?.Name,
                    HtmlLocalizedText = localizer?.HtmlText,
                    PublicationDateTimeUTC = p.PublicationDateTimeUTC,
                    Author = p.Author?.ResponseStructure,
                    PreviewImage = p.PreviewImage,
                    PreviewText = localizer?.PreviewText,
                    ModerationMessage = p.ModerationMessage,
                    ApprovalStatus = p.ApprovalStatus
                };
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

                var publishedPost = new PublishedPost
                {
                    Id = post.Id,
                    LocalizedName = localizer.Name,
                    HtmlLocalizedText = localizer.HtmlText,
                    PublicationDateTimeUTC = post.PublicationDateTimeUTC,
                    Author = post.Author?.ResponseStructure,
                    PreviewImage = post.PreviewImage,
                    PreviewText = localizer.PreviewText,
                    ApprovalStatus = post.ApprovalStatus
                };
                return Json(publishedPost);
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
                /*using (var binaryReader = new BinaryReader(postForm.PreviewImage.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int) postForm.PreviewImage.Length);
                }*/
                using (var ms = new MemoryStream())
                {
                    postForm.PreviewImage.CopyTo(ms);
                    imageData = ms.ToArray();
                }

                Post post = new Post
                {
                    PromotedDateTimeUTC = DateTime.UtcNow,
                    AuthorId = postForm.AuthorUserId,
                    PreviewImage = Convert.ToBase64String(imageData),
                    PostLocalizers = new List<PostLocalizer>()
                };
                /*
                var user = await HttpContext.GetCurrentUser(_userManager);
                if (user.IsLimitedInPosts)
                {
                    if (await _dbContext.Posts.AnyAsync(c => c.AuthorId == user.Id))
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
                }*/
                post.ApprovalStatus = ApproveType.Accepted;
                post.PublicationDateTimeUTC = DateTime.UtcNow;
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
                    message = ""
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
        public async Task<IActionResult> EditPost([FromBody] PostForm postForm, long id)
        {
            if (postForm.Id == null || id <= 0 || id != postForm.Id)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Id в запросе не совпадает с Id в форме" }
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
                        errors = new List<string> { "Попытка изменить несуществующий пост" }
                    });
                }
                else
                {
                    if ((await HttpContext.GetCurrentUser()).Id != post.AuthorId)
                    {
                        return Json(new
                        {
                            status = false,
                            errors = new List<string> { "Попытка изменить не свой пост" }
                        });
                    }

                    byte[] imageData = null;
                    /*using (var binaryReader = new BinaryReader(postForm.PreviewImage.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int) postForm.PreviewImage.Length);
                }*/
                    using (var ms = new MemoryStream())
                    {
                        postForm.PreviewImage.CopyTo(ms);
                        imageData = ms.ToArray();
                    }

                    post.AuthorId = postForm.AuthorUserId;
                    post.PreviewImage = Convert.ToBase64String(imageData);
                    post.PublicationDateTimeUTC = DateTime.UtcNow;
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
                            errors = new List<string> { "Ошибка параллельного сохранения" }
                        });
                    }

                    return Json(new
                    {
                        status = true,
                        message = ""
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

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpDelete("delete-post/{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            Post loadedPost = await _dbContext.Posts.FindAsync(id);
            if (loadedPost == null)
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Попытка удалить несуществующий пост" }
                });
            }

            var currentUser = await HttpContext.GetCurrentUser();
            var moderatorRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == RolesContainer.Moderator);
            if (currentUser.Id != loadedPost.AuthorId && !currentUser.Roles.Contains(moderatorRole))
            {
                return Json(new
                {
                    status = false,
                    errors = new List<string> { "Попытка удалить не свой пост или без админских прав" }
                });
            }

            _dbContext.Posts.Remove(loadedPost);
            await _dbContext.SaveChangesAsync();
            return Json(new
            {
                status = true,
                message = ""
            });
        }

        [HttpGet("get-post-requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetAllPostsRequests()
        {
            var posts = await _dbContext.Posts.ToListAsync();
            var requests = posts.ConvertAll(p =>
            {
                ConstructedPost pr = new ConstructedPost
                {
                    Id = p.Id,
                    Author = p.Author,
                    PromotedDateTimeUTC = p.PromotedDateTimeUTC,
                    ApprovalStatus = p.ApprovalStatus,
                    ApprovingModerator = p.ApprovingModerator,
                    Localizers = p.PostLocalizers,
                    ModerationMessage = p.ModerationMessage
                };
                return pr;
            });
            return Json(requests);
        }

        [HttpGet("get-post-requests/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetPostRequest(long id)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Такого поста не существует" }
                });
            }

            var request = new ConstructedPost
            {
                Id = post.Id,
                Author = post.Author,
                PromotedDateTimeUTC = post.PromotedDateTimeUTC,
                ApprovalStatus = post.ApprovalStatus,
                ApprovingModerator = post.ApprovingModerator,
                Localizers = post.PostLocalizers,
                ModerationMessage = post.ModerationMessage
            };
            return Json(Request);
        }

        [HttpPut("approve-post/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ApproveOrRejectPost([FromBody] PostRequestForm postRequestForm, long id)
        {
            if (postRequestForm.Id != id || id < 0)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Id в запросе не совпадает с Id в форме" }
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
                        errors = new List<string> { "Попытка модерировать несуществующий пост" }
                    });
                }
                else
                {
                    post.ApprovalStatus = postRequestForm.ApprovalStatus;
                    post.ApprovingModeratorId = postRequestForm.ApprovingModeratorId;
                    post.ModerationMessage = postRequestForm.ModerationMessage;
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
                            errors = new List<string> { "Ошибка параллельного сохранения" }
                        });
                    }

                    return Json(new
                    {
                        status = true,
                        message = ""
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