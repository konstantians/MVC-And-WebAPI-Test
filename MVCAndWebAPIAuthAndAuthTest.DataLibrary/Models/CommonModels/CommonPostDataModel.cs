using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.NoSqlModels;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.SqlModels;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.CommonModels;

public class CommonPostDataModel
{
    public string? Guid { get; set; }
    public DateTime SentAt { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? UserId { get; set; }

    public CommonPostDataModel(SqlPostDataModel sqlPostDataModel)
    {
        Guid = sqlPostDataModel.Guid;
        SentAt = sqlPostDataModel.SentAt;
        Title = sqlPostDataModel.Title;
        Content = sqlPostDataModel.Content;
        UserId = sqlPostDataModel.UserId;
    }

    public CommonPostDataModel(NoSqlPostDataModel noSqlPostDataModel)
    {
        Guid = noSqlPostDataModel.Guid;
        SentAt = noSqlPostDataModel.SentAt;
        Title = noSqlPostDataModel.Title;
        Content = noSqlPostDataModel.Content;
        UserId = noSqlPostDataModel.UserId;
    }
}
