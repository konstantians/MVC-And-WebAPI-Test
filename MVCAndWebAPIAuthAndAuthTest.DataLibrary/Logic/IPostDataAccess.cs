using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.CommonModels;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.CrudModels;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibrary.Logic;

public interface IPostDataAccess
{
    Task<string?> CreatePostAsync(CreatePostModel createPostModel);
    Task<bool> DeletePostAsync(string guid);
    Task<CommonPostDataModel?> GetPostAsync(string guid);
    Task<IEnumerable<CommonPostDataModel>> GetPostsAsync();
    Task<IEnumerable<CommonPostDataModel>> GetPostsAsync(int amount);
    Task<IEnumerable<CommonPostDataModel>> GetPostsOfUserAsync(string userId);
    Task<bool> UpdatePostAsync(string guid, EditPostModel editPostModel);
}