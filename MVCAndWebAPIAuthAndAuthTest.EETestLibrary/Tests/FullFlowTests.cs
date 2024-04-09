using Microsoft.Data.SqlClient;
using Microsoft.Playwright.NUnit;
using MVCAndWebAPIAuthAndAuthTest.EETestLibrary.HelperServices;
using MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Pages;
using MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Procedures;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    private LayoutPage _layoutPage;
    private SignUpPage _signUpPage;
    private SignInPage _signInPage;
    private ResetPasswordPage _resetPasswordPage;
    private EditAccountPage _editAccountPage;
    private IndexPage _indexPage;
    private IAccountTestHelperProcedures _testHelperProcedures;
    private IPostTestHelperProcedures _postTestHelperProcedures;

    [SetUp]
    public async Task Setup()
    {
        await Page.GotoAsync("https://localhost:44304/Home/Index");
        _layoutPage = new LayoutPage(Page);
        _signUpPage = new SignUpPage(Page);
        _signInPage = new SignInPage(Page);
        _resetPasswordPage = new ResetPasswordPage(Page);
        _editAccountPage = new EditAccountPage(Page);
        _indexPage = new IndexPage(Page);

        _testHelperProcedures = new AccountTestHelperProcedures(this, Page, _signUpPage, 
            _signInPage, _resetPasswordPage, _editAccountPage);
        _postTestHelperProcedures = new PostTestHelperProcedures(this, Page, _indexPage);
    }

    [Test]
    public async Task FullFlowTest()
    {
        await _testHelperProcedures.RegisterUser(clientError: true);
        await _testHelperProcedures.RegisterUser();
        await _layoutPage.LogUserOut();

        await _testHelperProcedures.RegisterUser(username: "konstantinos", serverErrorTestId: "duplicateUsername");
        await _testHelperProcedures.RegisterUser(username: "someone",
            email: "kinnaskonstantinos0@gmail.com", serverErrorTestId: "duplicateEmail");

        //create another user for future tests
        await _testHelperProcedures.RegisterUser(username: "konstantians", email: "kinnaskonstantinos@gmail.com");
        await _layoutPage.LogUserOut();

        await _testHelperProcedures.LoginUser(clientError: true);
        await _testHelperProcedures.LoginUser(username: "bogusUser", password: "BogusPass123@!", serverErrorTestId: "invalidCredentials");
        await _testHelperProcedures.LoginUser();
        await _layoutPage.LogUserOut();

        await _testHelperProcedures.ResetUserPassword(clientError: true);
        await _testHelperProcedures.ResetUserPassword(username: "BogusUser", serverErrorTestId: "falseResetAccount");
        await _testHelperProcedures.ResetUserPassword();

        await _testHelperProcedures.ChangeUserAccountBasicSettings(clientError: true);
        await _testHelperProcedures.ChangeUserAccountBasicSettings(username: "konstantians", serverErrorTestId: "duplicateUsernameError");
        await _testHelperProcedures.ChangeUserAccountBasicSettings();

        await _testHelperProcedures.ChangeUserAccountPassword(clientError: true);
        await _testHelperProcedures.ChangeUserAccountPassword(oldPassword: "Bogus22Pass@!", serverErrorTestId: "passwordMismatchError");
        await _testHelperProcedures.ChangeUserAccountPassword();

        await _testHelperProcedures.ChangeUserAccountEmail(clientError: true);
        await _testHelperProcedures.ChangeUserAccountEmail(newEmail: "kinnaskonstantinos@gmail.com", serverErrorTestId: "duplicateEmailError");
        await _testHelperProcedures.ChangeUserAccountEmail();

        //post stuff
        await _postTestHelperProcedures.CreatePost(clientError: true);
        await _postTestHelperProcedures.CreatePost();

        await _postTestHelperProcedures.EditPost(clientError: true);
        await _postTestHelperProcedures.EditPost();

        await _postTestHelperProcedures.DeletePost();
    }

    [TearDown]
    public async Task TearDown()
    {
        //The authentication database is always sql server
        ResetDatabaseService.ResetSqlAuthenticationDatabase();

        string databaseMode = Environment.GetEnvironmentVariable("DatabaseInUse")!;
        if(databaseMode == "SqlServer")
            ResetDatabaseService.ResetSqlDataDatabase();
        else
            await ResetDatabaseService.ResetNoSqlDatabase();
    }
}