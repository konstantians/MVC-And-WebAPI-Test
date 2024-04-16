namespace MVCAndWebAPIAuthAndAuthTest.EmailLibraryRestAPI.Tests.TestModels;

public class EmailTestModel
{
    public string? Id { get; set; }
    public string? Receiver { get; set; }
    public DateTime SentAt { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
}
