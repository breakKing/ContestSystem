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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ContestSystem.Services;
using ContestSystem.Models.Dictionaries;
using Microsoft.AspNetCore.Identity;
using ContestSystem.Models.Misc;

namespace ContestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : Controller
    {
        private readonly MainDbContext _dbContext;
        private readonly ILogger<PostsController> _logger;
        private readonly FileStorageService _storage;
        private readonly UserManager<User> _userManager;
        private readonly WorkspaceManagerService _workspace;

        private readonly string _entityName = Constants.PostEntityName;
        private readonly Dictionary<string, string> _errorCodes;

        public PostsController(MainDbContext dbContext, ILogger<PostsController> logger, FileStorageService storage,
            UserManager<User> userManager, WorkspaceManagerService workspace)
        {
            _dbContext = dbContext;
            _logger = logger;
            _storage = storage;
            _userManager = userManager;
            _workspace = workspace;

            _errorCodes = Constants.ErrorCodes[_entityName];
        }

        [HttpGet("{culture}")]
        public async Task<IActionResult> GetAllPublishedPosts(string culture)
        {
            var posts = await _dbContext.Posts.Where(p => p.ApprovalStatus == ApproveType.Accepted).ToListAsync();
            var localizers = posts.ConvertAll(post => post.PostLocalizers.FirstOrDefault(pl => pl.Culture == culture));
            var publishedPosts = new List<PublishedPost>();
            for (int i = 0; i < posts.Count; i++)
            {
                var pp = PublishedPost.GetFromModel(posts[i], localizers[i], _storage.GetImageInBase64(posts[i].ImagePath));
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
                var pp = PublishedPost.GetFromModel(p, localizer, _storage.GetImageInBase64(p.ImagePath));
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
                    return NotFound(_errorCodes[Constants.EntityLocalizerDoesntExistErrorName]);
                }

                var publishedPost = PublishedPost.GetFromModel(post, localizer, _storage.GetImageInBase64(post.ImagePath));
                return Json(publishedPost);
            }

            return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
        }

        [HttpGet("constructed/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        public async Task<IActionResult> GetConstructedPost(long id)
        {
            var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post != null)
            {
                var constructedPost = ConstructedPost.GetFromModel(post, _storage.GetImageInBase64(post.ImagePath));
                return Json(constructedPost);
            }
            return NotFound(_errorCodes[Constants.EntityDoesntExistErrorName]);
        }

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpPost("add-post")]
        public async Task<IActionResult> AddPost([FromForm] PostForm postForm)
        {
            var response = new ResponseObject<long>();

            if (ModelState.IsValid)
            {
                var currentUser = await HttpContext.GetCurrentUser(_userManager);

                if (currentUser.Id != postForm.AuthorUserId)
                {
                    _logger.LogCreationByNonEqualCurrentUserAndCreator(_entityName, currentUser.Id, postForm.AuthorUserId);
                    response = ResponseObject<long>.Fail(_errorCodes[Constants.UserIdMismatchErrorName]);
                }
                else
                {
                    CreationStatusData statusData = await _workspace.CreatePostAsync(_dbContext, postForm, currentUser.IsLimitedInPosts);
                    _logger.LogCreationStatus(statusData.Status, _entityName, statusData.Id, currentUser.Id);
                    response = ResponseObject<long>.FormResponseObjectForCreation(statusData.Status, _entityName, statusData.Id);
                }
            }
            else
            {
                response = ResponseObject<long>.Fail(ModelState, _entityName);
            }

            return Json(response);
        }

        [AuthorizeByJwt(Roles = RolesContainer.User)]
        [HttpPut("edit-post/{id}")]
        public async Task<IActionResult> EditPost([FromForm] PostForm postForm, long id)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);

            if (id != postForm.Id.GetValueOrDefault(-1))
            {
                _logger.LogEditingWithNonEqualFormAndRequestId(_entityName, postForm.Id, id, currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    Post post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
                    if (post == null)
                    {
                        _logger.LogEditingOfNonExistentEntity(_entityName, id, currentUser.Id);
                        response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        if (currentUser.Id != post.AuthorId)
                        {
                            _logger.LogEditingByNotAppropriateUser(_entityName, id, currentUser.Id);
                            response = ResponseObject<long>.Fail(_errorCodes[Constants.UserIdMismatchErrorName]);
                        }
                        else
                        {
                            EditionStatus status = await _workspace.EditPostAsync(_dbContext, postForm, post);
                            _logger.LogEditionStatus(status, _entityName, id, currentUser.Id);
                            response = ResponseObject<long>.FormResponseObjectForEdition(status, _entityName, id);
                        }
                    }
                }
                else
                {
                    response = ResponseObject<long>.Fail(ModelState, _entityName);
                }
            }

            return Json(response);
        }

        [AuthorizeByJwt(Roles = RolesContainer.Moderator + ", " + RolesContainer.User)]
        [HttpDelete("delete-post/{id}")]
        public async Task<IActionResult> DeletePost(long id)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            Post loadedPost = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if (loadedPost == null)
            {
                _logger.LogDeletingOfNonExistentEnitiy(_entityName, id, currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
            }
            else
            {
                if (currentUser.Id != loadedPost.AuthorId && !await _userManager.IsInRoleAsync(currentUser, RolesContainer.Moderator))
                {
                    _logger.LogDeletingByNotAppropriateUser(_entityName, id, currentUser.Id);
                    response = ResponseObject<long>.Fail(Constants.ErrorCodes[Constants.UserEntityName][Constants.UserInsufficientRightsErrorName]);
                }
                else
                {
                    DeletionStatus status = await _workspace.DeletePostAsync(_dbContext, loadedPost);
                    _logger.LogDeletionStatus(status, _entityName, id, currentUser.Id);
                    response = ResponseObject<long>.FormResponseObjectForDeletion(status, _entityName, id);
                }
            }

            return Json(response);
        }

        [HttpGet("get-requests")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetPostsRequests()
        {
            var posts = await _dbContext.Posts.Where(p => p.ApprovalStatus == ApproveType.NotModeratedYet).ToListAsync();
            var requests = posts.ConvertAll(p =>
            {
                ConstructedPost pr = ConstructedPost.GetFromModel(p, _storage.GetImageInBase64(p.ImagePath));
                return pr;
            });
            return Json(requests);
        }

        [HttpGet("get-approved")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetApprovedPosts()
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var posts = await _dbContext.Posts.Where(p => p.ApprovalStatus == ApproveType.Accepted 
                                                            && p.ApprovingModeratorId.GetValueOrDefault(-1) == currentUser.Id)
                                                .ToListAsync();
            var requests = posts.ConvertAll(p =>
            {
                ConstructedPost pr = ConstructedPost.GetFromModel(p, _storage.GetImageInBase64(p.ImagePath));
                return pr;
            });
            return Json(requests);
        }

        [HttpGet("get-rejected")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> GetRejectedPosts()
        {
            var currentUser = await HttpContext.GetCurrentUser(_userManager);
            var posts = await _dbContext.Posts.Where(p => p.ApprovalStatus == ApproveType.Rejected
                                                            && p.ApprovingModeratorId.GetValueOrDefault(-1) == currentUser.Id)
                                                .ToListAsync();
            var requests = posts.ConvertAll(p =>
            {
                ConstructedPost pr = ConstructedPost.GetFromModel(p, _storage.GetImageInBase64(p.ImagePath));
                return pr;
            });
            return Json(requests);
        }

        [HttpPut("moderate/{id}")]
        [AuthorizeByJwt(Roles = RolesContainer.Moderator)]
        public async Task<IActionResult> ApproveOrRejectPost([FromBody] PostRequestForm postRequestForm, long id)
        {
            var response = new ResponseObject<long>();
            var currentUser = await HttpContext.GetCurrentUser(_userManager);

            if (postRequestForm.PostId != id)
            {
                _logger.LogModeratingWithNonEqualFormAndRequestId(_entityName, postRequestForm.PostId, id, currentUser.Id);
                response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityIdMismatchErrorName]);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var post = await _dbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
                    if (post == null)
                    {
                        _logger.LogModeratingOfNonExistentEntity(_entityName, id, currentUser.Id);
                        response = ResponseObject<long>.Fail(_errorCodes[Constants.EntityDoesntExistErrorName]);
                    }
                    else
                    {
                        if (post.ApprovingModeratorId.GetValueOrDefault(-1) != currentUser.Id && post.ApprovalStatus != ApproveType.NotModeratedYet)
                        {
                            _logger.LogModeratingByWrongUser(_entityName, id, currentUser.Id, post.ApprovingModeratorId.GetValueOrDefault(-1), post.ApprovalStatus);
                            response = ResponseObject<long>.Fail(_errorCodes[Constants.ModerationByWrongModeratorErrorName]);
                        }
                        else
                        {
                            ModerationStatus status = await _workspace.ModeratePostAsync(_dbContext, postRequestForm, post);
                            _logger.LogModerationStatus(status, _entityName, id, currentUser.Id);
                            response = ResponseObject<long>.FormResponseObjectForModeration(status, _entityName, id);
                        }
                    }
                }
                else
                {
                    response = ResponseObject<long>.Fail(ModelState, _entityName);
                }
            }

            return Json(response);
        }
    }
}