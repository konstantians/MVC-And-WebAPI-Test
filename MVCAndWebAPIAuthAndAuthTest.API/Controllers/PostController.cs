using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCAndWebAPIAuthAndAuthTest.API.Models;
using MVCAndWebAPIAuthAndAuthTest.AuthLibrary;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary;
using MVCAndWebAPIAuthAndAuthTest.SharedModels;

namespace MVCAndWebAPIAuthAndAuthTest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IAuthenticationProcedures _authenticationProcedures;
    private readonly IPostDataAccess _postDataAccess;

    public PostController(IAuthenticationProcedures authenticationProcedures, IPostDataAccess postDataAccess)
    {
        _authenticationProcedures = authenticationProcedures;
        _postDataAccess = postDataAccess;
    }

    [AllowAnonymous]
    [HttpGet("GetPosts")]
    public async Task<IActionResult> GetPosts()
    {
        try
        {
            List<Post> posts = new List<Post>();
            var result = await _postDataAccess.GetPostsAsync(30);
            if (result is not null)
            {
                posts = result.ToList();
                foreach (Post post in posts)
                {
                    post.AppUser = await _authenticationProcedures.FindByUserIdAsync(post.UserId);
                }
            }

            return Ok(posts);
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [Authorize]
    [HttpPost("AddPost")]
    public async Task<IActionResult> AddPost(ApiCreatePostModel createPostModel)
    {
        try
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            IdentityUser? user = await _authenticationProcedures.GetCurrentUserByToken(token);
            if (user is null)
                return BadRequest(new { ErrorMessage = "InvalidToken"});

            Post post = new Post();
            post.Title = createPostModel.Title;
            post.Content = createPostModel.Content;
            post.SentAt = DateTime.Now;
            post.UserId = user.Id;

            var result = await _postDataAccess.CreatePostAsync(post);
            if (result == -1)
                return BadRequest(new { ErrorMessage = "FailedPostCreation" });

            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [Authorize]
    [HttpPost("EditPost")]
    public async Task<IActionResult> EditPost(ApiEditPostModel editPostModel)
    {
        try
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            IdentityUser? user = await _authenticationProcedures.GetCurrentUserByToken(token);
            if (user is null)
                return BadRequest(new { ErrorMessage = "InvalidToken" });

            var userPosts = await _postDataAccess.GetPostsOfUserAsync(user.Id);
            int postId = Convert.ToInt32(editPostModel.PostId);
            bool userOwnsPost = userPosts.ToList().Any(post => post.Id == postId);

            if (!userOwnsPost)
                return BadRequest(new { ErrorMessage = "UserDoesNotOwnPost" });

            Post post = new Post();
            post.Title = editPostModel.Title;
            post.Content = editPostModel.Content;
            post.SentAt = DateTime.Now;
            post.UserId = user.Id;

            var result = await _postDataAccess.UpdatePostAsync(postId, post);
            if (!result)
                return BadRequest(new {ErrorMessage = "FailedPostUpdate" });

            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [Authorize]
    [HttpPost("DeletePost")]
    public async Task<IActionResult> DeletePost(ApiDeletePostModel deletePostModel)
    {
        try
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            IdentityUser? user = await _authenticationProcedures.GetCurrentUserByToken(token);
            if (user is null)
                return BadRequest(new { ErrorMessage = "InvalidToken" });

            var userPosts = await _postDataAccess.GetPostsOfUserAsync(user.Id);
            int postId = Convert.ToInt32(deletePostModel.PostId);
            bool userOwnsPost = userPosts.ToList().Any(post => post.Id == postId);

            if (!userOwnsPost)
                return BadRequest(new { ErrorMessage = "UserDoesNotOwnPost" });

            var result = await _postDataAccess.DeletePostAsync(postId);
            if (!result)
                return BadRequest("failedPostDeletion");

            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }
}
