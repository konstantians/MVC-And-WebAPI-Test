using Microsoft.Playwright;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Pages;

public class LayoutPage
{
    private readonly IPage _page;
    private readonly ILocator _logOutButton; 
    public LayoutPage(IPage page)
    {
        _page = page;
        _logOutButton = _page.GetByTestId("logOutButton");
    }

    public async Task LogUserOut()
    {
        await _logOutButton.ClickAsync();
    }
}
