using System.ComponentModel.DataAnnotations;

namespace MVCAndWebAPIAuthAndAuthTest.MVC.Models.IndexModels;

public class EditPostModel : CreatePostModel
{
    [Required]
    public int PostId { get; set; }
}
