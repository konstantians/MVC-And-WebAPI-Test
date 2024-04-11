using Microsoft.Data.SqlClient;
using Microsoft.Playwright.NUnit;
using MVCAndWebAPIAuthAndAuthTest.EETestLibrary.HelperServices;
using MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Pages;
using MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Procedures;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]

public class PostActionsFlowTest : PageTest
{
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
        await _testHelperProcedures.RegisterUser();

        //post stuff
        await _postTestHelperProcedures.CreatePost(clientError: true);
        await _postTestHelperProcedures.CreatePost();

        await _postTestHelperProcedures.EditPost(clientError: true);
        await _postTestHelperProcedures.EditPost();

        //await _postTestHelperProcedures.DeletePost();
    }

    [TearDown]
    public async Task TearDown()
    {
        await ResetDatabaseService.DefaultResetDatabaseActions();
    }

}
