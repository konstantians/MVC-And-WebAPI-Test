namespace MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.NoSqlModels;

public class NoSqlPostDataModel
{
    public string? Guid { get; set; }
    public DateTime SentAt { get; set; }
    public string? Title { get; set; }
    public string? Content { get; set; }
    public string? UserId { get; set; }
}
