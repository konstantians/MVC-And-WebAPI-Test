using Microsoft.AspNetCore.Mvc;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Logic;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.RequestModels;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.ResponseModels;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibraryRestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DataPostController : ControllerBase
{
    private readonly IPostDataAccess _postDataAccess;

    public DataPostController(IPostDataAccess postDataAccess)
    {
        _postDataAccess = postDataAccess;
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts()
    {
        try
        {
            IEnumerable<PostResponseModel> result = await _postDataAccess.GetPostsAsync(30);

            return Ok(result.ToList());
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("{guid}")]
    public async Task<IActionResult> GetPost(string guid)
    {
        try
        {
            PostResponseModel? result = await _postDataAccess.GetPostAsync(guid);
            if (result is null)
                return NotFound();

            return Ok(result);
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddPost([FromBody] CreatePostRequestModel createPostModel)
    {
        try
        {
            string? createdEntityGuid = await _postDataAccess.CreatePostAsync(createPostModel);
            if (createdEntityGuid is null)
                return BadRequest(new { ErrorMessage = "FailedPostCreation" });

            PostResponseModel postResponseModel = new PostResponseModel()
            {
                Guid = createdEntityGuid,
                SentAt = DateTime.Now,
                UserId = createPostModel.UserId,
                Title = createPostModel.Title,
                Content = createPostModel.Content,
            };

            return CreatedAtAction(nameof(GetPost), new { guid = createdEntityGuid }, postResponseModel);
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPut]
    public async Task<IActionResult> EditPost([FromBody] EditPostRequestModel editPostModel)
    {
        try
        {
            IEnumerable<PostResponseModel> userPosts = await _postDataAccess.GetPostsOfUserAsync(editPostModel.UserId!);
            string guid = editPostModel.Guid!;
            bool userOwnsPost = userPosts.ToList().Any(post => post.Guid == guid);

            if (!userOwnsPost)
                return BadRequest(new { ErrorMessage = "UserDoesNotOwnPost" });

            var result = await _postDataAccess.UpdatePostAsync(guid, editPostModel);
            if (!result)
                return BadRequest(new { ErrorMessage = "FailedPostUpdate" });

            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpDelete("{guid}")]
    public async Task<IActionResult> DeletePost(string guid)
    {
        try
        {
            /*var userPosts = await _postDataAccess.GetPostsOfUserAsync(user.Id);
            bool userOwnsPost = userPosts.ToList().Any(post => post.Id == postId);

            if (!userOwnsPost)
                return BadRequest(new { ErrorMessage = "UserDoesNotOwnPost" });
            */

            bool result = await _postDataAccess.DeletePostAsync(guid);
            if (!result)
                return BadRequest(new { ErrorMessage = "FailedPostDeletion" });

            return NoContent();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }
}
