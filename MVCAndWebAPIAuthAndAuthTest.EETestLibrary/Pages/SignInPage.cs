using Microsoft.Playwright;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Pages;

public class SignInPage
{
    private readonly IPage _page;
    private readonly ILocator userLoginTitle;
    private readonly ILocator usernameField;
    private readonly ILocator passwordField;
    private readonly ILocator rememberMeCheckBox;
    private readonly ILocator signInButton;

    private readonly ILocator forgotPasswordLink;
    private readonly ILocator forgotPasswordUsernameField;
    private readonly ILocator forgotPasswordEmailField;
    private readonly ILocator forgotPasswordEmailOptionButton;
    private readonly ILocator forgotPasswordConfirmButton;

    public SignInPage(IPage page)
    {
        _page = page;

        userLoginTitle = _page.GetByTestId("userLoginTitle");

        usernameField = _page.GetByTestId("usernameInput");
        passwordField = _page.GetByTestId("passwordInput");
        rememberMeCheckBox = _page.GetByTestId("rememberMeInput");

        forgotPasswordLink = _page.GetByTestId("forgotPasswordLink");
        forgotPasswordUsernameField = _page.GetByTestId("forgotPasswordUsernameField");
        forgotPasswordEmailField = _page.GetByTestId("forgotPasswordEmailField");
        forgotPasswordEmailOptionButton = _page.GetByTestId("forgotPasswordEmailOptionButton");
        forgotPasswordConfirmButton = _page.GetByTestId("forgotPasswordConfirmButton");

        signInButton = _page.GetByTestId("signInButton");
    }

    public async Task NavigateToPage()
    {
        await _page.GotoAsync("https://localhost:44304/Account/SignIn");
    }

    public async Task SubmitSignInForm(string username = "konstantinos", string password = "Kinas2016!", 
        bool rememberMe = false, bool clientError = false)
    {
        await usernameField.FillAsync(username);
        if(rememberMe)
            await rememberMeCheckBox.CheckAsync();
 
        if (clientError)
            await passwordField.FillAsync("");
        else
            await passwordField.FillAsync(password);

        await signInButton.ClickAsync();
    }

    public async Task ForgotPassword(string username = "konstantinos", string email = "", bool clientError = false)
    {
        await forgotPasswordLink.ClickAsync();

        if (clientError)
        {
            await forgotPasswordConfirmButton.ClickAsync();
            return;
        }

        if ((username is null || username == "") && (email is null || email == ""))
            throw new AssertionException("Either the username or the email must be not null/empty");

        if (username != "" && username is not null)
        {
            await forgotPasswordUsernameField.FillAsync(username!);
            await forgotPasswordConfirmButton.ClickAsync();
            return;
        }

        await forgotPasswordEmailOptionButton.ClickAsync();
        await forgotPasswordEmailField.FillAsync(email);
        await forgotPasswordConfirmButton.ClickAsync();
    }

    public async Task<bool> IsPageShown()
    {
        return await userLoginTitle.IsVisibleAsync();
    }

}
