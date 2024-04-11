using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.InternalModels.SqlModels;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.RequestModels;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.ResponseModels;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibrary.Logic;

public class SqlPostDataAccess : IPostDataAccess
{
    private readonly SqlAppDbContext _context;
    private readonly ILogger<SqlPostDataAccess> _logger;

    public SqlPostDataAccess(SqlAppDbContext context, ILogger<SqlPostDataAccess> logger = null!)
    {
        _context = context;
        _logger = logger ?? NullLogger<SqlPostDataAccess>.Instance;
    }

    public async Task<IEnumerable<PostResponseModel>> GetPostsAsync()
    {
        try
        {
            List<SqlPostDataModel> sqlPostDataModels = await _context.Posts.ToListAsync();
            var postResponseModels = new List<PostResponseModel>();
            foreach (SqlPostDataModel sqlPostDataModel in sqlPostDataModels)
                postResponseModels.Add(new PostResponseModel(sqlPostDataModel));

            return postResponseModels;
        }
        catch (Exception ex)
        {
            _logger.LogError(2400, ex, "An error occurred while trying to retrieve application's posts. " +
                "ExceptionMessage: {ExceptionMessage}. StackTrace: {StackTrace}.", ex.Message, ex.StackTrace);
            throw;
        }
    }

    public async Task<IEnumerable<PostResponseModel>> GetPostsAsync(int amount)
    {
        try
        {
            List<SqlPostDataModel> sqlPostDataModels = await _context.Posts.OrderByDescending(post => post.SentAt)
                .Take(amount).ToListAsync();

            var postResponseModels = new List<PostResponseModel>();
            foreach (SqlPostDataModel sqlPostDataModel in sqlPostDataModels)
                postResponseModels.Add(new PostResponseModel(sqlPostDataModel));

            return postResponseModels;
        }
        catch (Exception ex)
        {

            _logger.LogError(2401, ex, "An error occurred while trying to retrieve application's posts with Amount: {Amount}. " +
                "ExceptionMessage: {ExceptionMessage}. StackTrace: {StackTrace}.", amount, ex.Message, ex.StackTrace);
            throw;
        }
    }

    public async Task<PostResponseModel?> GetPostAsync(string guid)
    {
        try
        {
            SqlPostDataModel? sqlPostDataModel = await _context.Posts.FirstOrDefaultAsync(post => post.Guid == guid);
            var postResponseModel = sqlPostDataModel is not null ? new PostResponseModel(sqlPostDataModel) : null;

            return postResponseModel;
        }
        catch (Exception ex)
        {
            _logger.LogError(2403, ex, "An error occurred while trying to retrieve post with PostId: {PostId}. " +
                "ExceptionMessage: {ExceptionMessage}. StackTrace: {StackTrace}.", guid, ex.Message, ex.StackTrace);
            throw;
        }
    }

    public async Task<IEnumerable<PostResponseModel>> GetPostsOfUserAsync(string userId)
    {
        try
        {
            List<SqlPostDataModel> sqlPostDataModels = await _context.Posts.Where(post => post.UserId == userId).ToListAsync();
            var postResponseModels = new List<PostResponseModel>();
            foreach (SqlPostDataModel sqlPostDataModel in sqlPostDataModels)
                postResponseModels.Add(new PostResponseModel(sqlPostDataModel));

            return postResponseModels;
        }
        catch (Exception ex)
        {
            _logger.LogError(2402, ex, "An error occurred while trying to retrieve users posts with UserId: {UserId}. " +
                "ExceptionMessage: {ExceptionMessage}. StackTrace: {StackTrace}.", userId, ex.Message, ex.StackTrace);
            throw;
        }
    }


    public async Task<string?> CreatePostAsync(CreatePostRequestModel createPostModel)
    {
        try
        {
            SqlPostDataModel sqlPostDataModel = new SqlPostDataModel(createPostModel);
            sqlPostDataModel.Guid = Guid.NewGuid().ToString();
            sqlPostDataModel.SentAt = DateTime.Now;

            var result = await _context.Posts.AddAsync(sqlPostDataModel);
            await _context.SaveChangesAsync();

            _logger.LogInformation(0400, "Successfully created post." +
                "PostId: {PostId}, SentAt: {SentAt}, Title: {Title}, Content: {Content}",
                result.Entity.Guid, DateTime.Now, createPostModel.Title, createPostModel.Content);
            return result.Entity.Guid;
        }
        catch (Exception ex)
        {
            _logger.LogError(2404, ex, "An error occurred while trying to create post. " +
                "SentAt: {SentAt}, Title: {Title}, Content: {Content}. " +
                "ExceptionMessage: {ExceptionMessage}. StackTrace: {StackTrace}.",
                DateTime.Now, createPostModel.Title, createPostModel.Content, ex.Message, ex.StackTrace);
            return null;
        }
    }

    public async Task<bool> UpdatePostAsync(string guid, EditPostRequestModel editPostModel)
    {
        try
        {
            SqlPostDataModel? foundPost = await _context.Posts.FirstOrDefaultAsync(post => post.Guid == editPostModel.Guid);

            if (foundPost is null)
            {
                _logger.LogWarning(1400, "Attempted to update null post, given PostId: {PostId}.", guid);
                return false;
            }

            foundPost.Title = editPostModel.Title;
            foundPost.Content = editPostModel.Content;
            await _context.SaveChangesAsync();

            _logger.LogInformation(0401, "Successfully updated post." +
                "PostId: {PostId}, Title: {Title}, Content: {Content}",
                guid, editPostModel.Title, editPostModel.Content);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(2405, ex, "An error occurred while trying to update post. " +
                "PostId: {PostId}, Title: {Title}, Content: {Content} " +
                "ExceptionMessage: {ExceptionMessage}. StackTrace: {StackTrace}.",
                guid, editPostModel.Title, editPostModel.Content, ex.Message, ex.StackTrace);
            return false;
        }
    }

    public async Task<bool> DeletePostAsync(string guid)
    {
        try
        {
            SqlPostDataModel? foundPost = await _context.Posts.FirstOrDefaultAsync(post => post.Guid == guid);
            if (foundPost is null)
            {
                _logger.LogWarning(1401, "Attempted to delete null post, given PostId: {PostId}.", guid);
                return false;
            }

            _context.Posts.Remove(foundPost);
            await _context.SaveChangesAsync();

            _logger.LogInformation(0402, "Successfully deleted post with PostId: {PostId}.", guid);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(2406, ex, "An error occurred while trying to delete post with PostId:{PostId}. " +
                "ExceptionMessage: {ExceptionMessage}. StackTrace: {StackTrace}.", guid, ex.Message, ex.StackTrace);
            return false;
        }
    }
}