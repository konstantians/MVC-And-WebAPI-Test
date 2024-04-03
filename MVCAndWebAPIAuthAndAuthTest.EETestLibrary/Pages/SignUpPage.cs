using Microsoft.Playwright;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Pages;

public class SignUpPage
{
    private readonly IPage _page;
    private readonly ILocator userRegisterTitle;
    private readonly ILocator usernameField;
    private readonly ILocator emailField;
    private readonly ILocator phoneNumberField;
    private readonly ILocator passwordField;
    private readonly ILocator repeatPasswordField;
    private readonly ILocator signUpButton;

    public SignUpPage(IPage page)
    {
        _page = page;

        userRegisterTitle = _page.GetByTestId("userRegisterTitle");

        usernameField = _page.GetByTestId("usernameInput");
        emailField = _page.GetByTestId("emailInput");
        phoneNumberField = _page.GetByTestId("phoneNumberInput");
        passwordField = _page.GetByTestId("passwordInput");
        repeatPasswordField = _page.GetByTestId("repeatPasswordInput");

        signUpButton = _page.GetByTestId("signUpButton");
    }

    public async Task NavigateToPage()
    {
        await _page.GotoAsync("https://localhost:44304/Account/SignUp");
    }


    public async Task SubmitRegisterForm(string username = "konstantinos", string email = "kinnaskonstantinos0@gmail.com",
        string phoneNumber = "6943655624", string password = "Kinas2016!", string repeatPassword = "Kinas2016!", bool clientError = false)
    {
        await usernameField.FillAsync(username);
        await emailField.FillAsync(email);
        await passwordField.FillAsync(password);
        await phoneNumberField.FillAsync(phoneNumber);
        if(clientError) 
            await repeatPasswordField.FillAsync(repeatPassword + "a");
        else
            await repeatPasswordField.FillAsync(repeatPassword);
    
        await signUpButton!.ClickAsync();
    }

    public async Task<bool> IsPageShown()
    {
        return await userRegisterTitle.IsVisibleAsync();
    }


}
