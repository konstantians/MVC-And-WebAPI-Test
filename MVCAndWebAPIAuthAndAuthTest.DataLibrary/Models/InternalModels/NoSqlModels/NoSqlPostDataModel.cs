using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.RequestModels;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.InternalModels.NoSqlModels;

internal class NoSqlPostDataModel
{
    public string? Guid { get; set; }
    public DateTime SentAt { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? UserId { get; set; }

    public NoSqlPostDataModel()
    {
        
    }

    public NoSqlPostDataModel(CreatePostRequestModel createPostRequestModel)
    {
        Title = createPostRequestModel.Title;
        Content = createPostRequestModel.Content;
        UserId = createPostRequestModel.UserId;
    }
}
