namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary
{
    public interface IPostTestHelperProcedures
    {
        Task CreatePost(string postTitle = "First Post", string postContent = "First Post Content", bool clientError = false);
        Task DeletePost();
        Task EditPost(string postTitle = "First Post Edited", string postContent = "First Post Content Edited", bool clientError = false);
    }
}