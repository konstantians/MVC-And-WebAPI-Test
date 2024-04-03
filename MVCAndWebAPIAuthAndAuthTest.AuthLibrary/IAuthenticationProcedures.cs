using Microsoft.AspNetCore.Identity;

namespace MVCAndWebAPIAuthAndAuthTest.AuthLibrary
{
    public interface IAuthenticationProcedures
    {
        Task<string> ChangeEmailAsync(string userId, string changeEmailToken, string newEmail);
        Task<(bool, string)> ChangePasswordAsync(IdentityUser appUser, string currentPassword, string newPassword);
        Task<string> ConfirmEmailAsync(string userId, string confirmationToken);
        Task<string> CreateChangeEmailTokenAsync(IdentityUser appUser, string newEmail);
        Task<string> CreateResetPasswordTokenAsync(IdentityUser appUser);
        Task<bool> DeleteUserAccountAsync(IdentityUser appUser);
        Task<IdentityUser> FindByEmailAsync(string email);
        Task<IdentityUser> FindByUserIdAsync(string userId);
        Task<IdentityUser> FindByUsernameAsync(string username);
        Task<IdentityUser?> GetCurrentUserByToken(string token);
        Task<List<IdentityUser>> GetUsersAsync();
        Task<(string, string)> RegisterUserAsync(IdentityUser appUser, string password, bool isPersistent);
        Task<string> ResetPasswordAsync(string userId, string resetPasswordToken, string newPassword);
        Task<string> SignInUserAsync(string username, string password, bool isPersistent);
        Task<bool> UpdateUserAccountAsync(IdentityUser appUser);
    }
}