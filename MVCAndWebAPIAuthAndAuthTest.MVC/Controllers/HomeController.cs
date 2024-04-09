using Microsoft.AspNetCore.Mvc;
using MVCAndWebAPIAuthAndAuthTest.MVC.Models;
using MVCAndWebAPIAuthAndAuthTest.MVC.ViewModels;
using MVCAndWebAPIAuthAndAuthTest.MVC.ViewModels.IndexViewModels;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MVCAndWebAPIAuthAndAuthTest.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient httpClient;


        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        public async Task<IActionResult> Index(bool successfulSignIn, bool failedAccountActivation, bool successfulAccountActivation,
            bool successfulPasswordReset, bool failedPasswordReset,
            bool successfulPostCreation, bool failedPostCreation,
            bool successfulPostUpdate, bool failedPostUpdate,
            bool successfulPostDeletion, bool failedPostDeletion)
        {
            var response = await httpClient.GetAsync("Post/GetPosts");

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return View("Error");

            var responseBody = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<List<PostModel>>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

            IndexViewModel indexViewModel = new IndexViewModel();
            indexViewModel.Posts = responseObject!;

            ViewData["SuccessfulSignIn"] = successfulSignIn;
            ViewData["FailedAccountActivation"] = failedAccountActivation;
            ViewData["SuccessfulAccountActivation"] = successfulAccountActivation;
            ViewData["SuccessfulPasswordReset"] = successfulPasswordReset;
            ViewData["FailedPasswordReset"] = failedPasswordReset;
            ViewData["SuccessfulPostCreation"] = successfulPostCreation;
            ViewData["FailedPostCreation"] = failedPostCreation;
            ViewData["SuccessfulPostUpdate"] = successfulPostUpdate;
            ViewData["FailedPostUpdate"] = failedPostUpdate;
            ViewData["SuccessfulPostDeletion"] = successfulPostDeletion;
            ViewData["FailedPostDeletion"] = failedPostDeletion;

            return View(indexViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(CreatePostViewModel createPostModel)
        {

            string? accessToken = Request.Cookies["SocialMediaAppAuthenticationCookie"];
            if (string.IsNullOrEmpty(accessToken))
                return View("Error");

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            var apiCreatePostModel = new Dictionary<string, string>
            {
                { "title", createPostModel.Title! },
                { "content", createPostModel.Content! }
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.PostAsJsonAsync("Post/AddPost", apiCreatePostModel);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return View("Error");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                if (responseObject is null)
                    return View("Error");
                
                responseObject!.TryGetValue("errorMessage", out string? errorMessage);

                if (errorMessage == "InvalidToken")
                {
                    Response.Cookies.Delete("SocialMediaAppAuthenticationCookie");
                    return RedirectToAction("Account", "Login");
                }

                return RedirectToAction("Index", "Home", new { failedPostCreation = true });
            }

            return RedirectToAction("Index", "Home", new { successfulPostCreation = true });
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(EditPostViewModel editPostModel)
        {
            string? accessToken = Request.Cookies["SocialMediaAppAuthenticationCookie"];
            if (string.IsNullOrEmpty(accessToken))
                return View("Error");

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            var apiEditPostModel = new Dictionary<string, string>
            {
                { "guid", editPostModel.Guid! },
                { "title", editPostModel.Title! },
                { "content", editPostModel.Content! }
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.PostAsJsonAsync("Post/EditPost", apiEditPostModel);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return View("Error");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                if (responseObject is null)
                    return View("Error");

                responseObject!.TryGetValue("errorMessage", out string? errorMessage);

                if (errorMessage == "InvalidToken")
                {
                    Response.Cookies.Delete("SocialMediaAppAuthenticationCookie");
                    return RedirectToAction("Account", "Login");
                }
                else if (errorMessage == "UserDoesNotOwnPost")
                    return RedirectToAction("Index", "Home");

                return RedirectToAction("Index", "Home", new { failedPostUpdate = true });
            }
            
            return RedirectToAction("Index", "Home", new { successfulPostUpdate = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(string guid)
        {
            string? accessToken = Request.Cookies["SocialMediaAppAuthenticationCookie"];
            if (string.IsNullOrEmpty(accessToken))
                return View("Error");

            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Home");
            }

            var apiDeletePostModel = new Dictionary<string, string>
            {
                { "guid", guid }
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.PostAsJsonAsync("Post/DeletePost", apiDeletePostModel);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
                return View("Error");

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
                if (responseObject is null)
                    return View("Error");

                responseObject!.TryGetValue("errorMessage", out string? errorMessage);

                if (errorMessage == "InvalidToken")
                {
                    Response.Cookies.Delete("SocialMediaAppAuthenticationCookie");
                    return RedirectToAction("Account", "Login");
                }
                else if (errorMessage == "UserDoesNotOwnPost")
                    return RedirectToAction("Index", "Home");

                return RedirectToAction("Index", "Home", new { failedPostDeletion = true });
            }

            return RedirectToAction("Index", "Home", new { successfulPostDeletion = true });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}