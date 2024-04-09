﻿using Microsoft.AspNetCore.Mvc;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Logic;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.CrudModels;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.SqlModels;

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
            List<SqlPostDataModel> posts = new List<SqlPostDataModel>();
            var result = await _postDataAccess.GetPostsAsync(30);

            return Ok(result.ToList());
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddPost([FromBody] CreatePostModel createPostModel)
    {
        try
        {
            var result = await _postDataAccess.CreatePostAsync(createPostModel);
            if (result is null)
                return BadRequest(new { ErrorMessage = "FailedPostCreation" });

            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPut]
    public async Task<IActionResult> EditPost([FromBody] EditPostModel editPostModel)
    {
        try
        {
            var userPosts = await _postDataAccess.GetPostsOfUserAsync(editPostModel.UserId!);
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

            var result = await _postDataAccess.DeletePostAsync(guid);
            if (!result)
                return BadRequest(new { ErrorMessage = "FailedPostDeletion" });

            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }
}
