namespace MVCAndWebAPIAuthAndAuthTest.API.RequestModels;

public class ApiResetPasswordModel
{
    public string? UserId { get; set; }
    public string? Token { get; set; }
    public string? Password { get; set; }
}
