using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;
using MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Pages;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary;

public class PostTestHelperProcedures : IPostTestHelperProcedures
{
    private readonly PlaywrightTest _playWright;
    private readonly IPage _page;
    private readonly IndexPage _indexPage;

    public PostTestHelperProcedures(PlaywrightTest playwright, IPage page, IndexPage indexPage)
    {
        _playWright = playwright;
        _page = page;
        _indexPage = indexPage;
    }

    public async Task CreatePost(string postTitle = "First Post", string postContent = "First Post Content", bool clientError = false)
    {
        await _indexPage.NavigateToPage();
        Assert.IsTrue(await _indexPage.IsPageShown());

        await _indexPage.AddPost(postTitle, postContent, clientError);

        if (clientError)
            return;

        ILocator successfulPostCreationAlert = _page.GetByTestId("successfulPostCreation");
        await _playWright.Expect(successfulPostCreationAlert).ToBeVisibleAsync();
    }

    public async Task EditPost(string postTitle = "First Post Edited", string postContent = "First Post Content Edited", bool clientError = false)
    {
        await _indexPage.NavigateToPage();
        Assert.IsTrue(await _indexPage.IsPageShown());

        await _indexPage.EditPost(postTitle, postContent, clientError);

        if (clientError)
            return;

        ILocator successfulPostUpdateAlert = _page.GetByTestId("successfulPostUpdate");
        await _playWright.Expect(successfulPostUpdateAlert).ToBeVisibleAsync();
    }

    public async Task DeletePost()
    {
        await _indexPage.NavigateToPage();
        Assert.IsTrue(await _indexPage.IsPageShown());

        await _indexPage.DeletePost();

        ILocator successfulPostDeletionAlert = _page.GetByTestId("successfulPostDeletion");
        await _playWright.Expect(successfulPostDeletionAlert).ToBeVisibleAsync();
    }
}
