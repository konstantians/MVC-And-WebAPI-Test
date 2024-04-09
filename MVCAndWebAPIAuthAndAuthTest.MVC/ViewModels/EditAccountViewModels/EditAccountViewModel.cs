namespace MVCAndWebAPIAuthAndAuthTest.MVC.ViewModels.EditAccountViewModels;

public class EditAccountViewModel
{
    public AccountBasicSettingsViewModel AccountBasicSettingsViewModel { get; set; } = new();
    public ChangePasswordViewModel ChangePasswordModel { get; set; } = new();
    public ChangeEmailViewModel ChangeEmailModel { get; set; } = new();
}
