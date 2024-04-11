using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCAndWebAPIAuthAndAuthTest.API.Models.RequestModels;
using MVCAndWebAPIAuthAndAuthTest.API.Models.ResponseModels;
using MVCAndWebAPIAuthAndAuthTest.AuthLibrary;
using System.Net;
using System.Text.Json;

namespace MVCAndWebAPIAuthAndAuthTest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IAuthenticationProcedures _authenticationProcedures;
    private readonly HttpClient httpClient;

    public PostController(IAuthenticationProcedures authenticationProcedures, IHttpClientFactory httpClientFactory)
    {
        _authenticationProcedures = authenticationProcedures;
        httpClient = httpClientFactory.CreateClient("DataAccessRestApiClient"); 
    }

    [AllowAnonymous]
    [HttpGet("GetPosts")]
    public async Task<IActionResult> GetPosts()
    {
        try
        {
            var response = await httpClient.GetAsync("DataPost");

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return StatusCode(500, "Internal Server Error");

            var responseBody = await response.Content.ReadAsStringAsync();
            List<ApiPostResponseModel> apiPostModels = JsonSerializer.Deserialize<List<ApiPostResponseModel>>(
                responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

            foreach (ApiPostResponseModel apiPostModel in apiPostModels)
                apiPostModel.AppUser = await _authenticationProcedures.FindByUserIdAsync(apiPostModel.UserId!);

            return Ok(apiPostModels);
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [Authorize]
    [HttpPost("AddPost")]
    public async Task<IActionResult> AddPost(ApiCreatePostRequestModel createPostModel)
    {
        try
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            IdentityUser? user = await _authenticationProcedures.GetCurrentUserByToken(token);
            if (user is null)
                return BadRequest(new { ErrorMessage = "InvalidToken"});

            var apiCreatePostModel = new Dictionary<string, string>
            {
                { "title", createPostModel.Title! },
                { "content", createPostModel.Content! },
                { "userId", user.Id! }
            };

            var response = await httpClient.PostAsJsonAsync("DataPost", apiCreatePostModel);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return StatusCode(500, "Internal Server Error");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                if (responseObject is null)
                    return BadRequest();

                return BadRequest(new { ErrorMessage = "FailedPostCreation" });
            }

            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [Authorize]
    [HttpPost("EditPost")]
    public async Task<IActionResult> EditPost(ApiEditPostRequestModel editPostModel)
    {
        try
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            IdentityUser? user = await _authenticationProcedures.GetCurrentUserByToken(token);
            if (user is null)
                return BadRequest(new { ErrorMessage = "InvalidToken" });

            var apiEditPostModel = new Dictionary<string, string>
            {
                { "guid", editPostModel.Guid! },
                { "title", editPostModel.Title! },
                { "content", editPostModel.Content! },
                { "userId", user.Id }
            };

            var response = await httpClient.PutAsJsonAsync("DataPost", apiEditPostModel);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return StatusCode(500, "Internal Server Error");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                if (responseObject is null)
                    return BadRequest();

                responseObject!.TryGetValue("errorMessage", out string? errorMessage);

                if (errorMessage == "UserDoesNotOwnPost")
                    return BadRequest(new { ErrorMessage = "UserDoesNotOwnPost" });

                return BadRequest(new { ErrorMessage = "FailedPostUpdate" });
            }

            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [Authorize]
    [HttpPost("DeletePost")]
    public async Task<IActionResult> DeletePost(ApiDeletePostRequestModel deletePostModel)
    {
        try
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            IdentityUser? user = await _authenticationProcedures.GetCurrentUserByToken(token);
            if (user is null)
                return BadRequest(new { ErrorMessage = "InvalidToken" });

            var response = await httpClient.DeleteAsync($"DataPost/{deletePostModel.Guid}");

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return StatusCode(500, "Internal Server Error");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                if (responseObject is null)
                    return BadRequest();

                responseObject!.TryGetValue("errorMessage", out string? errorMessage);

                if (errorMessage == "UserDoesNotOwnPost")
                    return BadRequest(new { ErrorMessage = "UserDoesNotOwnPost" });

                return BadRequest(new { ErrorMessage = "FailedPostDeletion" });
            }

            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }
}
