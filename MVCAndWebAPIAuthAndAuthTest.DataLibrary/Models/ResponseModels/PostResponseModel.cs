using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.InternalModels.NoSqlModels;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.InternalModels.SqlModels;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.ResponseModels;

public class PostResponseModel
{
    public string? Guid { get; set; }
    public DateTime SentAt { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? UserId { get; set; }

    public PostResponseModel(){ }
    internal PostResponseModel(SqlPostDataModel sqlPostDataModel)
    {
        Guid = sqlPostDataModel.Guid;
        SentAt = sqlPostDataModel.SentAt;
        Title = sqlPostDataModel.Title;
        Content = sqlPostDataModel.Content;
        UserId = sqlPostDataModel.UserId;
    }

    internal PostResponseModel(NoSqlPostDataModel noSqlPostDataModel)
    {
        Guid = noSqlPostDataModel.Guid;
        SentAt = noSqlPostDataModel.SentAt;
        Title = noSqlPostDataModel.Title;
        Content = noSqlPostDataModel.Content;
        UserId = noSqlPostDataModel.UserId;
    }
}
