namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Procedures;

public interface IAccountTestHelperProcedures
{
    Task ChangeUserAccountBasicSettings(string username = "konstantinos58", string phoneNumber = "6911111111",
        bool clientError = false, string serverErrorTestId = "");
    Task ChangeUserAccountEmail(string newEmail = "realag58@gmail.com", bool clientError = false, string serverErrorTestId = "");
    Task ChangeUserAccountPassword(string oldPassword = "Kinas2016!", string newPassword = "Kinas2020!",
        string confirmNewPassword = "Kinas2020!", bool clientError = false, string serverErrorTestId = "");
    Task LoginUser(string username = "konstantinos", string password = "Kinas2016!",
        bool rememberMe = false, bool clientError = false, string serverErrorTestId = "");
    Task RegisterUser(string username = "konstantinos", string email = "kinnaskonstantinos0@gmail.com",
        string phoneNumber = "6943655624", string password = "Kinas2016!",
        string repeatPassword = "Kinas2016!", bool clientError = false, string serverErrorTestId = "");
    Task ResetUserPassword(string username = "konstantinos", string email = "", string password = "Kinas2016!",
        string repeatPassword = "Kinas2016!", bool clientError = false, string serverErrorTestId = "");
}