using MVCAndWebAPIAuthAndAuthTest.MVC.Models;

namespace MVCAndWebAPIAuthAndAuthTest.MVC.ViewModels.IndexViewModels;

public class IndexViewModel
{
    public List<PostModel> Posts { get; set; } = new List<PostModel>();
    public CreatePostViewModel CreatePostModel { get; set; } = new CreatePostViewModel();
    public EditPostViewModel EditPostModel { get; set; } = new EditPostViewModel();
}
