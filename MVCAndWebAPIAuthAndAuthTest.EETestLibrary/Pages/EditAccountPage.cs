using Microsoft.Playwright;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Pages;

public class EditAccountPage
{
    private readonly IPage _page;
    private readonly ILocator editAccountTitle;
    private readonly ILocator usernameEditButton;
    private readonly ILocator usernameInput;
    private readonly ILocator phoneNumberEditButton;
    private readonly ILocator phoneNumberInput;
    private readonly ILocator editBasicSettingsConfirmButton;

    private readonly ILocator newPasswordEditModalButton;
    private readonly ILocator oldPasswordInput;
    private readonly ILocator newPasswordInput;
    private readonly ILocator confirmNewPasswordInput;
    private readonly ILocator newPasswordConfirmModalButton;

    private readonly ILocator newEmailEditModalButton;
    private readonly ILocator newEmailInput;
    private readonly ILocator newEmailConfirmModalButton;

    public EditAccountPage(IPage page)
    {
        _page = page;
        editAccountTitle = _page.GetByTestId("editAccountTitle");
        
        usernameInput = _page.GetByTestId("usernameInput");
        usernameEditButton = _page.GetByTestId("usernameEditButton");
        phoneNumberInput = _page.GetByTestId("phoneNumberInput");
        phoneNumberEditButton = _page.GetByTestId("phoneNumberEditButton");
        editBasicSettingsConfirmButton = _page.GetByTestId("editBasicSettingsConfirmButton");

        newEmailEditModalButton = _page.GetByTestId("newEmailEditModalButton");
        newEmailInput = _page.GetByTestId("newEmailInput");
        newEmailConfirmModalButton = _page.GetByTestId("newEmailConfirmModalButton");

        newPasswordEditModalButton = _page.GetByTestId("newPasswordEditModalButton");
        oldPasswordInput = _page.GetByTestId("oldPasswordInput");
        newPasswordInput = _page.GetByTestId("newPasswordInput");
        confirmNewPasswordInput = _page.GetByTestId("confirmNewPasswordInput");
        newPasswordConfirmModalButton = _page.GetByTestId("newPasswordConfirmModalButton");
    }

    public async Task NavigateToPage()
    {
        await _page.GotoAsync("https://localhost:44304/Account/EditAccount");
    }

    public async Task ChangeAccountBasicSettings(string username = "konstantinos58", string phoneNumber = "6911111111", bool clientError = false)
    {
        await usernameEditButton.ClickAsync();
        if (clientError)
            await usernameInput.FillAsync("");
        else    
            await usernameInput.FillAsync(username);
        await usernameEditButton.ClickAsync();

        await phoneNumberEditButton.ClickAsync();
        if (clientError)
            await phoneNumberInput.FillAsync("");
        else
            await phoneNumberInput.FillAsync(phoneNumber);
        await phoneNumberEditButton.ClickAsync();

        await editBasicSettingsConfirmButton.ClickAsync();
    }

    public async Task ChangeAccountPassword(string oldPassword = "Kinas2016!", string newPassword = "Kinas2020!", 
        string confirmNewPassword = "Kinas2020!", bool clientError = false)
    {
        await newPasswordEditModalButton.ClickAsync();
        if (clientError)
        {
            await oldPasswordInput.FillAsync("");
            await newPasswordInput.FillAsync("");
            await confirmNewPasswordInput.FillAsync("");
        }
        else
        {
            await oldPasswordInput.FillAsync(oldPassword);
            await newPasswordInput.FillAsync(newPassword);
            await confirmNewPasswordInput.FillAsync(confirmNewPassword);
        }

        await newPasswordConfirmModalButton.ClickAsync();
    }

    public async Task ChangeAccountEmail(string newEmail = "realag58@gmail.com", bool clientError = false)
    {
        await newEmailEditModalButton.ClickAsync();
        if (clientError)
            await newEmailInput.FillAsync("");
        else
            await newEmailInput.FillAsync(newEmail);

        await newEmailConfirmModalButton.ClickAsync();
    }

    public async Task<bool> IsPageShown()
    {
        return await editAccountTitle.IsVisibleAsync();
    }
}
