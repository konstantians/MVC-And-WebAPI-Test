using Microsoft.AspNetCore.Mvc;
using MVCAndWebAPIAuthAndAuthTest.MVC.Models.EditAccountModels;
using System.Net;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using MVCAndWebAPIAuthAndAuthTest.MVC.Models;

namespace MVCAndWebAPIAuthAndAuthTest.MVC.Controllers;

public class AccountController : Controller
{
    private readonly HttpClient httpClient;

    public AccountController(IHttpClientFactory httpClientFactory)
    {
        httpClient = httpClientFactory.CreateClient("ApiClient");
    }

    [HttpGet]
    public IActionResult SignUp(bool duplicateUsername, bool duplicateEmail)
    {
        //if the cookie is null that means that the user is not signed in
        if (!string.IsNullOrEmpty(Request.Cookies["SocialMediaAppAuthenticationCookie"]))
            return RedirectToAction("Index", "Home");

        ViewData["DuplicateUsername"] = duplicateUsername;
        ViewData["DuplicateEmail"] = duplicateEmail;

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(RegisterModel registerModel)
    {
        if (!string.IsNullOrEmpty(Request.Cookies["SocialMediaAppAuthenticationCookie"]))
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
        {
            ViewData["DuplicateUsername"] = false;
            ViewData["DuplicateEmail"] = false;
            return View();
        }

        var apiSignUpModel = new Dictionary<string, string>
        {
            { "username", registerModel.Username! },
            { "email", registerModel.Email! },
            { "password", registerModel.Password! },
            { "phoneNumber", registerModel.PhoneNumber! }
        };

        var response = await httpClient.PostAsJsonAsync("Authentication/SignUp", apiSignUpModel);

        if (response.StatusCode == HttpStatusCode.InternalServerError)
            return View("Error");

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);

        if (response.StatusCode == HttpStatusCode.BadRequest && 
            responseObject != null && responseObject.TryGetValue("errorMessage", out string? errorMessage))
        {
            if(errorMessage == "AlreadyAuthenticatedUser")
                return RedirectToAction("Index", "Home");
            else if (errorMessage == "DuplicateUsername")
                return SignUp(true, false);
            else if (errorMessage == "DuplicateEmail")
                return SignUp(false, true);
        }
        
        if (responseObject != null && responseObject.TryGetValue("warning", out string? warningMessage))
            ViewData["EmailSendSuccessfully"] = warningMessage == "None";

        return View("RegisterVerificationEmailMessage");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (!string.IsNullOrEmpty(Request.Cookies["SocialMediaAppAuthenticationCookie"]))
            return RedirectToAction("Index", "Home");

        var response = await httpClient.GetAsync($"Authentication/ConfirmEmail?userId={userId}&token={WebUtility.UrlEncode(token)}");

        if (response.StatusCode == HttpStatusCode.InternalServerError)
            return View("Error");

        if(response.StatusCode == HttpStatusCode.BadRequest)
            return RedirectToAction("Index", "Home", new { FailedAccountActivation = true });

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
        if (responseObject != null && responseObject.TryGetValue("accessToken", out string? accessToken))
            SetUpAuthenticationCookie(accessToken);

        return RedirectToAction("Index", "Home", new { SuccessfulAccountActivation = true });
    }

    [HttpGet]
    public IActionResult SignIn(bool falseResetAccount, bool invalidCredentials)
    {
        if(Request.Cookies["SocialMediaAppAuthenticationCookie"] is not null)
            return RedirectToAction("Index", "Home");

        ViewData["FalseResetAccount"] = falseResetAccount;
        ViewData["InvalidCredentials"] = invalidCredentials;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(SignInModel signInModel)
    {
        if (!string.IsNullOrEmpty(Request.Cookies["SocialMediaAppAuthenticationCookie"]))
            return RedirectToAction("Index", "Home");

        if (!ModelState.IsValid)
            return RedirectToAction("SignIn", "Account");

        var apiSignInModel = new Dictionary<string, string>
        {
            { "username", signInModel.Username! },
            { "password", signInModel.Password! },
            { "rememberMe", signInModel.RememberMe.ToString()! }
        };

        var response = await httpClient.PostAsJsonAsync("Authentication/SignIn", apiSignInModel);

        if (response.StatusCode == HttpStatusCode.InternalServerError)
            return View("Error");

        if (response.StatusCode == HttpStatusCode.Unauthorized)
            return RedirectToAction("SignIn", "Account", new { invalidCredentials = true });

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
        if (responseObject != null && responseObject.TryGetValue("accessToken", out string? accessToken))
            SetUpAuthenticationCookie(accessToken, signInModel.RememberMe ? 30 : 0);

        return RedirectToAction("Index", "Home", new { SuccessfulSignIn = true });
    }

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(string username, string email)
    {
        if (!string.IsNullOrEmpty(Request.Cookies["SocialMediaAppAuthenticationCookie"]))
            return RedirectToAction("Index", "Home");

        var apiForgotPasswordModel = new Dictionary<string, string>
        {
            { "username", username ?? ""},
            { "email", email ?? ""}
        };

        var response = await httpClient.PostAsJsonAsync("Authentication/ForgotPassword", apiForgotPasswordModel);

        if (response.StatusCode == HttpStatusCode.InternalServerError)
            return View("Error");

        if (response.StatusCode == HttpStatusCode.BadRequest)
            return RedirectToAction("SignIn", "Account", new { falseResetAccount = true });

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
        if (responseObject != null && responseObject.TryGetValue("warning", out string? warning))
            ViewData["EmailSendSuccessfully"] = warning == "None" ? true : false;

        return View("ResetPasswordEmailMessage");
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string userId, string token)
    {
        if (!string.IsNullOrEmpty(Request.Cookies["SocialMediaAppAuthenticationCookie"]))
            return RedirectToAction("Index", "Home");

        var response = await httpClient.GetAsync($"Authentication/ResetPassword?userId={userId}");

        if(response.StatusCode == HttpStatusCode.InternalServerError)
            return View("Error");

        if(response.StatusCode == HttpStatusCode.BadRequest)
            return RedirectToAction("Index", "Home");

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
        if (responseObject != null && responseObject.TryGetValue("username", out string? username))
            ViewData["Username"] = username;

        ViewData["UserId"] = userId;
        ViewData["Token"] = token;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPasswordModel)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        if (!string.IsNullOrEmpty(Request.Cookies["SocialMediaAppAuthenticationCookie"]))
            return RedirectToAction("Index", "Home");

        string decodedToken = WebUtility.UrlDecode(resetPasswordModel.Token)!;
        decodedToken = decodedToken.Replace(" ", "+");
        var apiResetPasswordModel = new Dictionary<string, string>
        {
            { "userId", resetPasswordModel.UserId! },
            { "token", decodedToken! },
            { "password", resetPasswordModel.Password! }
        };

        var response = await httpClient.PostAsJsonAsync("Authentication/ResetPassword", apiResetPasswordModel);

        if (response.StatusCode == HttpStatusCode.InternalServerError)
            return View("Error");

        if(response.StatusCode == HttpStatusCode.BadRequest)
            return RedirectToAction("Index", "Home", new { failedPasswordReset = true });

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
        if (responseObject != null && responseObject.TryGetValue("accessToken", out string? accessToken))
            SetUpAuthenticationCookie(accessToken);

        return RedirectToAction("Index", "Home", new { successfulPasswordReset = true });
    }

    [HttpGet]
    public async Task<IActionResult> EditAccount(bool duplicateUsernameError, bool duplicateEmailError,
        bool basicInformationChangeError, bool basicInformationChangeSuccess,
        bool passwordChangeSuccess, bool passwordChangeError, bool passwordMismatchError)
    {
        string? accessToken = Request.Cookies["SocialMediaAppAuthenticationCookie"];
        if (string.IsNullOrEmpty(accessToken))
            return View("Error");

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.GetAsync("Authentication/EditAccount");

        if (response.StatusCode == HttpStatusCode.InternalServerError)
            return View("Error");
        
        if(response.StatusCode == HttpStatusCode.BadRequest)
        {
            Response.Cookies.Delete("SocialMediaAppAuthenticationCookie");
            return RedirectToAction("Account", "Login");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        IdentityUser user = JsonSerializer.Deserialize<IdentityUser>(responseBody, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        AccountBasicSettingsViewModel accountBasicSettingsViewModel = new()
        {
            Username = user.UserName,
            PhoneNumber = user.PhoneNumber,
        };

        ChangePasswordModel changePasswordModel = new()
        {
            OldPassword = user.PasswordHash
        };

        ChangeEmailModel resetEmailModel = new()
        {
            OldEmail = user.Email
        };

        EditAccountModel editAccountModel = new()
        {
            AccountBasicSettingsViewModel = accountBasicSettingsViewModel,
            ChangePasswordModel = changePasswordModel,
            ChangeEmailModel = resetEmailModel
        };

        ViewData["DuplicateUsernameError"] = duplicateUsernameError;
        ViewData["DuplicateEmailError"] = duplicateEmailError;

        ViewData["BasicInformationChangeError"] = basicInformationChangeError;
        ViewData["BasicInformationChangeSuccess"] = basicInformationChangeSuccess;

        ViewData["PasswordChangeSuccess"] = passwordChangeSuccess;
        ViewData["PasswordChangeError"] = passwordChangeError;
        ViewData["PasswordMismatchError"] = passwordMismatchError;

        return View(editAccountModel);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeBasicAccountSettings(AccountBasicSettingsViewModel accountBasicSettingsViewModel)
    {
        string? accessToken = Request.Cookies["SocialMediaAppAuthenticationCookie"];
        if (string.IsNullOrEmpty(accessToken))
            return View("Error");

        if (!ModelState.IsValid)
        {
            return View();
        }

        var apiChangeAccountBasicSettingsModel = new Dictionary<string, string>
        {
            { "username", accountBasicSettingsViewModel.Username! },
            { "phoneNumber", accountBasicSettingsViewModel.PhoneNumber! }
        };

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.PostAsJsonAsync("Authentication/ChangeBasicAccountSettings", apiChangeAccountBasicSettingsModel);

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
            else if(errorMessage == "DuplicateUsername")
                return RedirectToAction("EditAccount", "Account", new { duplicateUsernameError = true });

            return RedirectToAction("EditAccount", "Account", new { basicInformationChangeError = true });
        }

        return RedirectToAction("EditAccount", "Account", new { basicInformationChangeSuccess = true });
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
    {
        string? accessToken = Request.Cookies["SocialMediaAppAuthenticationCookie"];
        if (string.IsNullOrEmpty(accessToken))
            return View("Error");

        if (!ModelState.IsValid)
        {
            return RedirectToAction("EditAccount", "Account", new { passwordChangeError = true });
        }

        var apiChangePasswordModel = new Dictionary<string, string>
        {
            { "oldPassword", changePasswordModel.OldPassword! },
            { "newPassword", changePasswordModel.NewPassword! }
        };

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.PostAsJsonAsync("Authentication/ChangePassword", apiChangePasswordModel);

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
            else if (errorMessage == "PasswordMismatchError")
                return RedirectToAction("EditAccount", "Account", new { passwordMismatchError = true });

            return RedirectToAction("EditAccount", "Account", new { passwordChangeError = true });
        }

        return RedirectToAction("EditAccount", "Account", new { passwordChangeSuccess = true });
    }

    [HttpPost]
    public async Task<IActionResult> RequestChangeAccountEmail(ChangeEmailModel changeEmailModel)
    {
        string? accessToken = Request.Cookies["SocialMediaAppAuthenticationCookie"];
        if (string.IsNullOrEmpty(accessToken))
            return View("Error");

        if (!ModelState.IsValid)
        {
            return RedirectToAction("EditAccount", "Account");
        }

        var apiChangeEmailModel = new Dictionary<string, string>
        {
            { "oldEmail", changeEmailModel.OldEmail! },
            { "newEmail", changeEmailModel.NewEmail! }
        };

        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await httpClient.PostAsJsonAsync("Authentication/RequestChangeAccountEmail", apiChangeEmailModel);

        if (response.StatusCode == HttpStatusCode.InternalServerError)
            return View("Error");

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
        if (responseObject is null)
            return View("Error");
        responseObject!.TryGetValue("errorMessage", out string? errorMessage);
        responseObject!.TryGetValue("warning", out string? warningMessage);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            if (errorMessage == "InvalidToken")
            {
                Response.Cookies.Delete("SocialMediaAppAuthenticationCookie");
                return RedirectToAction("Account", "Login");
            }

            return RedirectToAction("EditAccount", "Account", new { duplicateEmailError = true });
        }

        //log out the user
        Response.Cookies.Delete("SocialMediaAppAuthenticationCookie");
        ViewData["EmailSendSuccessfully"] = warningMessage == "None" ? true : false;
        return View("EmailChangeVerificationMessage");
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmChangeEmail(string userId, string newEmail, string token)
    {
        if (!string.IsNullOrEmpty(Request.Cookies["SocialMediaAppAuthenticationCookie"]))
            return RedirectToAction("Index", "Home");

        var response = await httpClient.GetAsync(
            $"Authentication/ConfirmChangeEmail?userId={userId}&newEmail={newEmail}&token={WebUtility.UrlEncode(token)}");

        if (response.StatusCode == HttpStatusCode.InternalServerError)
            return View("Error");

        if (response.StatusCode == HttpStatusCode.BadRequest)
            return RedirectToAction("Index", "Home", new { FailedAccountActivation = true });

        var responseBody = await response.Content.ReadAsStringAsync();
        var responseObject = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
        if (responseObject != null && responseObject.TryGetValue("accessToken", out string? accessToken))
            SetUpAuthenticationCookie(accessToken);

        return RedirectToAction("Index", "Home", new { SuccessfulAccountActivation = true });
    }

    [HttpPost]
    public IActionResult LogOut()
    {
        string? accessToken = Request.Cookies["SocialMediaAppAuthenticationCookie"];
        if (string.IsNullOrEmpty(accessToken))
            return View("Error");

        Response.Cookies.Delete("SocialMediaAppAuthenticationCookie");
        return RedirectToAction("Index", "Home");
    }

    private void SetUpAuthenticationCookie(string accessToken, int duration = 0)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            IsEssential = true
        };

        //no value means that the cookie will be destroyed when the browser closes
        if (duration != 0)
            cookieOptions.Expires = DateTimeOffset.Now.AddDays(30);

        Response.Cookies.Append("SocialMediaAppAuthenticationCookie", accessToken, cookieOptions);
        
    }
}
