using Microsoft.Playwright;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Pages;

public class ResetPasswordPage
{
    private readonly IPage _page;
    private readonly ILocator passwordResetTitle;
    private readonly ILocator passwordField;
    private readonly ILocator repeatPasswordField;
    private readonly ILocator passwordResetConfirmButton;

    public ResetPasswordPage(IPage page)
    {
        _page = page;

        passwordResetTitle = _page.GetByTestId("passwordResetTitle");

        passwordField = _page.GetByTestId("passwordInput");
        repeatPasswordField = _page.GetByTestId("repeatPasswordInput");

        passwordResetConfirmButton = _page.GetByTestId("passwordResetConfirmButton");
    }


    public async Task SubmitResetPasswordForm(string password = "Kinas2016!", string repeatPassword="Kinas2016!", bool clientError = false)
    {
        await passwordField.FillAsync(password);
        if (clientError)
            await repeatPasswordField.FillAsync(repeatPassword + "a");
        await repeatPasswordField.FillAsync(repeatPassword);

        await passwordResetConfirmButton.ClickAsync();
    }

    public async Task<bool> IsPageShown()
    {
        return await passwordResetTitle.IsVisibleAsync();
    }

}
