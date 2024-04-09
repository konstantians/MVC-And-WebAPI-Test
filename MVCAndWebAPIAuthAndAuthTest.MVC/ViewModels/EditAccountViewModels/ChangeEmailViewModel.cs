using System.ComponentModel.DataAnnotations;

namespace MVCAndWebAPIAuthAndAuthTest.MVC.ViewModels.EditAccountViewModels;

public class ChangeEmailViewModel
{
    [Required(ErrorMessage = "This field is required")]
    public string? OldEmail { get; set; }

    [Required(ErrorMessage = "This field is required")]
    [EmailAddress]
    public string? NewEmail { get; set; }

}
