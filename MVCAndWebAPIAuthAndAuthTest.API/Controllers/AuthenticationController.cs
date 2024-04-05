using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVCAndWebAPIAuthAndAuthTest.API.Models;
using MVCAndWebAPIAuthAndAuthTest.AuthLibrary;
using MVCAndWebAPIAuthAndAuthTest.EmailLibrary;
using System.Net;

namespace MVCAndWebAPIAuthAndAuthTest.API.Controllers;

[ApiController]
[Route("api/[controller]")]

public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationProcedures _authenticationProcedures;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _configuration;

    public AuthenticationController(IAuthenticationProcedures authenticationProcedures, IEmailService emailService, IConfiguration configuration)
    {
        _authenticationProcedures = authenticationProcedures;
        _emailService = emailService;
        _configuration = configuration;
    }


    [HttpGet("TryGetCurrentUser")]
    [AllowAnonymous]
    public async Task<IActionResult> TryGetCurrentUser()
    {
        // Retrieve the Authorization header from the HTTP request
        string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
        string token = authorizationHeader.Substring("Bearer ".Length).Trim();

        IdentityUser? user = await _authenticationProcedures.GetCurrentUserByToken(token);
        return Ok(user);
    }

    [HttpPost("SignUp")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] ApiSignUpModel signUpModel)
    {
        try
        {
            IdentityUser user = await _authenticationProcedures.FindByUsernameAsync(signUpModel.Username!);
            if (user is not null)
                return BadRequest(new { ErrorMessage = "DuplicateUsername"});

            user = await _authenticationProcedures.FindByEmailAsync(signUpModel.Email!);
            if (user is not null)
                return BadRequest(new { ErrorMessage = "DuplicateEmail" });

            user = new IdentityUser();
            user.UserName = signUpModel.Username!;
            user.PhoneNumber = signUpModel.PhoneNumber!;
            user.Email = signUpModel.Email;

            (string userId, string confirmationToken) = await _authenticationProcedures.RegisterUserAsync(user, signUpModel.Password!, false);

            //maybe do a check here
            string message = "Click on the following link to confirm your email:";
            string link = $"{_configuration["WebClientOriginUrl"]}/Account/ConfirmEmail?userId={userId}&token={WebUtility.UrlEncode(confirmationToken)}";
            string? confirmationLink = $"{message} {link}";

            var emailSentResult = await _emailService.SendEmailAsync(user.Email!, "Email Confirmation", confirmationLink);
            if (!emailSentResult)
                return Ok(new {Warning = "ConfirmationEmailNotSent"});

            return Ok(new {Warning = "None"});
        }
        catch
        {
            return StatusCode(500, "Internal Server");
        }
    }

    [HttpGet("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        try
        {
            string accessToken = await _authenticationProcedures.ConfirmEmailAsync(userId, token);
            if (accessToken is null)
                return BadRequest();

            return Ok(new {AccessToken = accessToken});
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("SignIn")]
    [AllowAnonymous]
    public async Task<IActionResult> SignIn([FromBody] ApiSignInModel signInModel)
    {
        try
        {
            string accessToken = await _authenticationProcedures.
                SignInUserAsync(signInModel.Username!, signInModel.Password!, signInModel.RememberMe == "True");
            
            if(accessToken is null)
                return Unauthorized();

            return Ok(new {AccessToken = accessToken});
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }      
    }

    [HttpPost("ForgotPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ApiForgotPasswordModel forgotPasswordModel)
    {
        try
        {
            IdentityUser user;
            if (forgotPasswordModel.Username is not null || forgotPasswordModel.Username != "")
                user = await _authenticationProcedures.FindByUsernameAsync(forgotPasswordModel.Username!);
            else
                user = await _authenticationProcedures.FindByEmailAsync(forgotPasswordModel.Email!);

            if (user is null)
                return BadRequest();

            string resetToken = await _authenticationProcedures.CreateResetPasswordTokenAsync(user);

            string message = "Click on the following link to reset your account password:";
            string? link = $"{_configuration["WebClientOriginUrl"]}/Account/ResetPassword?userId={user.Id}&token={WebUtility.UrlEncode(resetToken)}";
            string? confirmationLink = $"{message} {link}";

            bool result = await _emailService.SendEmailAsync(user.Email!, "Email Confirmation", confirmationLink);
            if (!result)
                return Ok(new { Warning = "ResetEmailNotSent" });

            return Ok(new { Warning = "None"});
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("ResetPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword(string userId)
    {
        try
        {
            IdentityUser user = await _authenticationProcedures.FindByUserIdAsync(userId);
            if(user is null)
                return BadRequest();

            return Ok(new { Username = user.UserName});
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("ResetPassword")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ApiResetPasswordModel resetPasswordModel)
    {
        try
        {
            string accessToken = await _authenticationProcedures.ResetPasswordAsync(
                    resetPasswordModel.UserId!, resetPasswordModel.Token!, resetPasswordModel.Password!);

            if (accessToken == null)
                return BadRequest();

            return Ok(new { AccessToken = accessToken });
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet("EditAccount")]
    [Authorize]
    public async Task<IActionResult> EditAccount()
    {
        try
        {
            // Retrieve the Authorization header from the HTTP request
            string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            IdentityUser? user = await _authenticationProcedures.GetCurrentUserByToken(token);
            if(user is null)
                return BadRequest();

            return Ok(user);
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("ChangeBasicAccountSettings")]
    [Authorize]
    public async Task<IActionResult> ChangeBasicAccountSettings([FromBody] ApiAccountBasicSettingsViewModel accountBasicSettingsViewModel)
    {
        try
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            IdentityUser? user = await _authenticationProcedures.GetCurrentUserByToken(token);
            if (user is null)
                return BadRequest(new { ErrorMessage = "InvalidToken"});

            IdentityUser otherUser = await _authenticationProcedures.FindByUsernameAsync(accountBasicSettingsViewModel.Username!);
            
            //there is another given with the given username
            if (otherUser is not null && otherUser.Email != user.Email)
                return BadRequest(new { ErrorMessage = "DuplicateUsername"});

            user.UserName = accountBasicSettingsViewModel.Username;
            user.PhoneNumber = accountBasicSettingsViewModel.PhoneNumber;
            bool result = await _authenticationProcedures.UpdateUserAccountAsync(user);
            if(!result)
                return BadRequest(new { ErrorMessage = "BasicInformationChangeError" });

            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("ChangePassword")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ApiChangePasswordModel changePasswordModel)
    {
        try
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            IdentityUser? user = await _authenticationProcedures.GetCurrentUserByToken(token);
            if (user is null)
                return BadRequest(new { ErrorMessage = "InvalidToken" });

            (bool result, string errorCode) = await _authenticationProcedures.ChangePasswordAsync(
                user, changePasswordModel.OldPassword!, changePasswordModel.NewPassword!);

            if (!result && errorCode == "passwordMismatch")
                return BadRequest(new { ErrorMessage = "PasswordMismatchError" });
            else if (!result)
                return BadRequest(new { ErrorMessage = "PasswordChangeError" });

            return Ok();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost("RequestChangeAccountEmail")]
    [Authorize]
    public async Task<IActionResult> RequestChangeAccountEmail([FromBody] ApiChangeEmailModel changeEmailModel)
    {
        try
        {
            string authorizationHeader = HttpContext.Request.Headers["Authorization"]!;
            string token = authorizationHeader.Substring("Bearer ".Length).Trim();

            IdentityUser? user = await _authenticationProcedures.GetCurrentUserByToken(token);
            if (user is null)
                return BadRequest(new { ErrorMessage = "InvalidToken" });

            IdentityUser otherUser = await _authenticationProcedures.FindByEmailAsync(changeEmailModel.NewEmail!);
            if (otherUser is not null)
                return BadRequest(new { ErrorMessage = "DuplicateEmail" });

            string resetToken = await _authenticationProcedures.CreateChangeEmailTokenAsync(user, changeEmailModel.NewEmail!);

            string message = "Click on the following link to confirm your account's new email:";
            string? link = 
                $"{_configuration["WebClientOriginUrl"]}/Account/ConfirmChangeEmail?userId={user.Id}&newEmail={changeEmailModel.NewEmail}&token={WebUtility.UrlEncode(resetToken)}";

            string? confirmationLink = $"{message} {link}";
            bool result = await _emailService.SendEmailAsync(changeEmailModel.NewEmail!, "Email Change Confirmation", confirmationLink);
            if (!result)
                return Ok(new { Warning = "EmailNotSent" });

            user.EmailConfirmed = false;
            await _authenticationProcedures.UpdateUserAccountAsync(user);
            return Ok(new { Warning = "None" });
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }


    [HttpGet("ConfirmChangeEmail")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmChangeEmail(string userId, string newEmail, string token)
    {
        try
        {
            string accessToken = await _authenticationProcedures.ChangeEmailAsync(userId, token, newEmail);
            if (accessToken is null)
                return BadRequest();
            return Ok(new {AccessToken = accessToken});
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }
}
