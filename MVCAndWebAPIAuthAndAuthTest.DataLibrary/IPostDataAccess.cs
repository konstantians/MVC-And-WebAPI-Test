using MVCAndWebAPIAuthAndAuthTest.SharedModels;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibrary
{
    public interface IPostDataAccess
    {
        Task<int> CreatePostAsync(Post post);
        Task<bool> DeletePostAsync(int id);
        Task<Post> GetPostAsync(int id);
        Task<IEnumerable<Post>> GetPostsAsync();
        Task<IEnumerable<Post>> GetPostsAsync(int amount);
        Task<IEnumerable<Post>> GetPostsOfUserAsync(string userId);
        Task<bool> UpdatePostAsync(int postId, Post post);
    }
}