using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using MVCAndWebAPIAuthAndAuthTest.EETestLibrary.HelperServices;
using MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Pages;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Procedures;

public class AccountTestHelperProcedures : IAccountTestHelperProcedures
{
    private readonly PlaywrightTest _playWright;
    private readonly IPage _page;
    private SignUpPage _signUpPage;
    private SignInPage _signInPage;
    private ResetPasswordPage _resetPasswordPage;
    private EditAccountPage _editAccountPage;

    public AccountTestHelperProcedures(PlaywrightTest playwright, IPage page, SignUpPage signUpPage, SignInPage signInPage,
        ResetPasswordPage resetPasswordPage, EditAccountPage editAccountPage)
    {
        _playWright = playwright;
        _page = page;
        _signUpPage = signUpPage;
        _signInPage = signInPage;
        _resetPasswordPage = resetPasswordPage;
        _editAccountPage = editAccountPage;
    }

    public async Task RegisterUser(string username = "konstantinos", string email = "kinnaskonstantinos0@gmail.com",
        string phoneNumber = "6943655624", string password = "Kinas2016!",
        string repeatPassword = "Kinas2016!", bool clientError = false, string serverErrorTestId = "")
    {
        //go to the sign up page
        await _signUpPage.NavigateToPage();
        Assert.IsTrue(await _signUpPage.IsPageShown());

        await _signUpPage.SubmitRegisterForm(username, email, phoneNumber, password, repeatPassword, clientError);

        if (clientError)
            return;

        if (serverErrorTestId != "")
        {
            ILocator failureAlert = _page.GetByTestId(serverErrorTestId);
            await _playWright.Expect(failureAlert).ToBeVisibleAsync();
            return;
        }

        // expect to get to the confirmation page
        await _playWright.Expect(_page).ToHaveURLAsync("https://localhost:44304/Account/SignUp");

        await Task.Delay(1000);

        string lastEmailLink = EmailReaderService.GetLastEmailLink()!;
        if (lastEmailLink is null)
            throw new AssertionException("The email could not be read.");

        await _page.GotoAsync(lastEmailLink);

        await _playWright.Expect(_page).ToHaveURLAsync("https://localhost:44304/?SuccessfulAccountActivation=True");

        ILocator successAlert = _page.GetByTestId("successfulOperationAlert");
        await _playWright.Expect(successAlert).ToBeVisibleAsync();
    }

    public async Task LoginUser(string username = "konstantinos", string password = "Kinas2016!",
        bool rememberMe = false, bool clientError = false, string serverErrorTestId = "")
    {
        await _signInPage.NavigateToPage();
        Assert.IsTrue(await _signInPage.IsPageShown());

        await _signInPage.SubmitSignInForm(username, password, rememberMe, clientError);

        if (clientError)
            return;

        if (serverErrorTestId != "")
        {
            ILocator failureAlert = _page.GetByTestId(serverErrorTestId);
            await _playWright.Expect(failureAlert).ToBeVisibleAsync();
            return;
        }

        await _playWright.Expect(_page).ToHaveURLAsync("https://localhost:44304/?SuccessfulSignIn=True");

    }

    public async Task ResetUserPassword(string username = "konstantinos", string email = "", string password = "Kinas2016!",
        string repeatPassword = "Kinas2016!", bool clientError = false, string serverErrorTestId = "")
    {
        await _signInPage.NavigateToPage();
        Assert.IsTrue(await _signInPage.IsPageShown());

        await _signInPage.ForgotPassword(username, email, clientError);

        if (clientError)
        {
            //EmailReaderService.DeleteLastEmailLink();
            return;
        }

        if (serverErrorTestId != "")
        {
            ILocator failureAlert = _page.GetByTestId(serverErrorTestId);
            await _playWright.Expect(failureAlert).ToBeVisibleAsync();
            return;
        }

        await Task.Delay(1000);

        string lastEmailLink = EmailReaderService.GetLastEmailLink()!;
        if (lastEmailLink is null)
            throw new AssertionException("The email could not be read.");

        await _page.GotoAsync(lastEmailLink);

        Assert.IsTrue(await _resetPasswordPage.IsPageShown());

        await _resetPasswordPage.SubmitResetPasswordForm(password, repeatPassword, clientError);
        await _playWright.Expect(_page).ToHaveURLAsync("https://localhost:44304/?successfulPasswordReset=True");
    }

    public async Task ChangeUserAccountBasicSettings(string username = "konstantinos58", string phoneNumber = "6911111111",
        bool clientError = false, string serverErrorTestId = "")
    {
        await _editAccountPage.NavigateToPage();
        Assert.IsTrue(await _editAccountPage.IsPageShown());

        await _editAccountPage.ChangeAccountBasicSettings(username, phoneNumber, clientError);

        if (clientError)
            return;

        if (serverErrorTestId != "")
        {
            ILocator failureAlert = _page.GetByTestId(serverErrorTestId);
            await _playWright.Expect(failureAlert).ToBeVisibleAsync();
            return;
        }

        ILocator changeAccountBasicSettingsSuccessAlert = _page.GetByTestId("changeAccountBasicSettingsSuccessAlert");
        await _playWright.Expect(changeAccountBasicSettingsSuccessAlert).ToBeVisibleAsync();
    }

    public async Task ChangeUserAccountPassword(string oldPassword = "Kinas2016!", string newPassword = "Kinas2020!",
        string confirmNewPassword = "Kinas2020!", bool clientError = false, string serverErrorTestId = "")
    {
        await _editAccountPage.NavigateToPage();
        Assert.IsTrue(await _editAccountPage.IsPageShown());

        await _editAccountPage.ChangeAccountPassword(oldPassword, newPassword, confirmNewPassword, clientError);

        if (clientError)
            return;

        if (serverErrorTestId != "")
        {
            ILocator failureAlert = _page.GetByTestId(serverErrorTestId);
            await _playWright.Expect(failureAlert).ToBeVisibleAsync();
            return;
        }

        ILocator changeAccountPasswordSuccessAlert = _page.GetByTestId("changeAccountPasswordSuccessAlert");
        await _playWright.Expect(changeAccountPasswordSuccessAlert).ToBeVisibleAsync();

    }

    public async Task ChangeUserAccountEmail(string newEmail = "realag58@gmail.com",
        bool clientError = false, string serverErrorTestId = "")
    {
        await _editAccountPage.NavigateToPage();
        Assert.IsTrue(await _editAccountPage.IsPageShown());

        await _editAccountPage.ChangeAccountEmail(newEmail, clientError);

        if (clientError)
            return;

        if (serverErrorTestId != "")
        {
            ILocator failureAlert = _page.GetByTestId(serverErrorTestId);
            await _playWright.Expect(failureAlert).ToBeVisibleAsync();
            return;
        }

        // expect to get to the confirmation page
        await _playWright.Expect(_page).ToHaveURLAsync("https://localhost:44304/Account/RequestChangeAccountEmail");

        await Task.Delay(1000);

        string lastEmailLink = EmailReaderService.GetLastEmailLink()!;
        if (lastEmailLink is null)
            throw new AssertionException("The email could not be read.");

        await _page.GotoAsync(lastEmailLink);

        await _playWright.Expect(_page).ToHaveURLAsync("https://localhost:44304/?SuccessfulAccountActivation=True");

        ILocator successAlert = _page.GetByTestId("successfulOperationAlert");
        await _playWright.Expect(successAlert).ToBeVisibleAsync();
    }
}
