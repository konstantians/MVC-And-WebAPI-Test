using System.ComponentModel.DataAnnotations;

namespace MVCAndWebAPIAuthAndAuthTest.MVC.ViewModels.IndexViewModels;

public class EditPostViewModel : CreatePostViewModel
{
    [Required]
    public string? Guid { get; set; }
}
