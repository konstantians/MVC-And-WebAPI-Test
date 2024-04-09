using System.ComponentModel.DataAnnotations;

namespace MVCAndWebAPIAuthAndAuthTest.MVC.ViewModels.IndexViewModels;

public class CreatePostViewModel
{
    [Required]
    [StringLength(40, MinimumLength = 1, ErrorMessage = "The Post Title Can Not Exceed 40 Characters")]
    public string? Title { get; set; }
    [Required]
    [StringLength(500, MinimumLength = 1, ErrorMessage = "The Post Content Can Not Exceed 500 Characters")]
    public string? Content { get; set; }
}
