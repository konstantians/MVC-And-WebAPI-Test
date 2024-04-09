using System.ComponentModel.DataAnnotations;

namespace MVCAndWebAPIAuthAndAuthTest.MVC.ViewModels;

public class SignInViewModel
{
    [Required(ErrorMessage = "This field is required")]
    public string? Username { get; set; }
    [Required(ErrorMessage = "This field is required")]
    public string? Password { get; set; }
    public bool RememberMe { get; set; }
}
