using Microsoft.Playwright;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Pages;

public class IndexPage
{
    private readonly IPage _page;
    private readonly ILocator homeTitle;
    private readonly ILocator addPostButton;
    private readonly ILocator editPostButtonFirst;
    private readonly ILocator deletePostButtonFirst;

    private readonly ILocator addPostTitleInput;
    private readonly ILocator addPostContentInput;
    private readonly ILocator addPostConfirmButton;

    private readonly ILocator editPostTitleInputFirst;
    private readonly ILocator editPostContentInputFirst;
    private readonly ILocator editPostConfirmButtonFirst;

    private readonly ILocator deletePostConfirmButtonFirst;
    
    private readonly ILocator successfulPostCreationAlert;

    public IndexPage(IPage page)
    {
        _page = page;

        homeTitle = _page.GetByTestId("homeTitle");

        addPostButton = _page.GetByTestId("addPostButton");
        editPostButtonFirst = _page.GetByTestId("editPostButton-0");
        deletePostButtonFirst = _page.GetByTestId("deletePostButton-0");

        addPostTitleInput = _page.GetByTestId("addPostTitleInput");
        addPostContentInput = _page.GetByTestId("addPostContentInput");
        addPostConfirmButton = _page.GetByTestId("addPostConfirmButton");

        editPostTitleInputFirst = _page.GetByTestId("editPostTitleInput-0");
        editPostContentInputFirst = _page.GetByTestId("editPostContentInput-0");
        editPostConfirmButtonFirst = _page.GetByTestId("editPostConfirmButton-0");

        deletePostConfirmButtonFirst = _page.GetByTestId("deletePostConfirmButton-0");

        successfulPostCreationAlert = _page.GetByTestId("successfulPostCreation");
    }

    public async Task NavigateToPage()
    {
        await _page.GotoAsync("https://localhost:44304/");
    }

    public async Task AddPost(string postTitle = "First Post", string postContent= "First Post Content", bool clientError = false)
    {
        await addPostButton.ClickAsync();
        if(clientError)
        {
            await addPostTitleInput.FillAsync("");
            await addPostContentInput.FillAsync("");
        }
        else
        {
            await addPostTitleInput.FillAsync(postTitle);
            await addPostContentInput.FillAsync(postContent);
        }
        await addPostConfirmButton.ClickAsync();


    }

    public async Task EditPost(string postTitle = "First Post Edited", string postContent = "First Post Content Edited", bool clientError = false)
    {
        await editPostButtonFirst.ClickAsync();
        if (clientError)
        {
            await editPostTitleInputFirst.FillAsync("");
            await editPostContentInputFirst.FillAsync("");
        }
        else
        {
            await editPostTitleInputFirst.FillAsync(postTitle);
            await editPostContentInputFirst.FillAsync(postContent);
        }
        await editPostConfirmButtonFirst.ClickAsync();
    }

    public async Task DeletePost()
    {
        await deletePostButtonFirst.ClickAsync();
        await deletePostConfirmButtonFirst.ClickAsync();
    }

    public async Task<bool> IsPageShown()
    {
        return await homeTitle.IsVisibleAsync();
    }
}
