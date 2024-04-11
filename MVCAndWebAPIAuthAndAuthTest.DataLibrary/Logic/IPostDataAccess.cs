using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.RequestModels;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.ResponseModels;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibrary.Logic;

public interface IPostDataAccess
{
    Task<string?> CreatePostAsync(CreatePostRequestModel createPostModel);
    Task<bool> DeletePostAsync(string guid);
    Task<PostResponseModel?> GetPostAsync(string guid);
    Task<IEnumerable<PostResponseModel>> GetPostsAsync();
    Task<IEnumerable<PostResponseModel>> GetPostsAsync(int amount);
    Task<IEnumerable<PostResponseModel>> GetPostsOfUserAsync(string userId);
    Task<bool> UpdatePostAsync(string guid, EditPostRequestModel editPostModel);
}