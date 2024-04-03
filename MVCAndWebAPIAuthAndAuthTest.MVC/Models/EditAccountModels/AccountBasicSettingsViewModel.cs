using System.ComponentModel.DataAnnotations;

namespace MVCAndWebAPIAuthAndAuthTest.MVC.Models.EditAccountModels;

public class AccountBasicSettingsViewModel
{
    [Required(ErrorMessage = "This field is required")]
    public string? Username { get; set; }

    public string? AccountType { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [Phone]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }
}
