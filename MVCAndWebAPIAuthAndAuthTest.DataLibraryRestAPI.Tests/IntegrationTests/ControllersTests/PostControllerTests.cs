using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using MVCAndWebAPIAuthAndAuthTest.DataLibraryRestAPI.Tests.HelperMethods;
using MVCAndWebAPIAuthAndAuthTest.DataLibraryRestAPI.Tests.TestModels;
using System.Net;
using System.Net.Http.Json;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibraryRestAPI.Tests.IntegrationTests.ControllersTests;

[TestFixture]
[Category("Integration")]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
public class PostControllerTests
{
    private HttpClient httpClient;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>();
        httpClient = webApplicationFactory.CreateClient();
    }

    [Test, Order(1)]
    public async Task AddPost_ShouldCreatePostAndReturnCreated()
    {
        //Arrange
        var createPostModel = new Dictionary<string, string>();
        createPostModel.TryAdd("UserId", "userTestId");
        createPostModel.TryAdd("Title", "Test Title");
        createPostModel.TryAdd("Content", "test content");

        //Act
        var response = await httpClient.PostAsJsonAsync("/api/dataPost", createPostModel);
        PostTestModel? postTestModel = await response.Content.ReadFromJsonAsync<PostTestModel>();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        postTestModel.Should().NotBeNull();
        postTestModel!.Guid.Should().Be(postTestModel.Guid);
    }

    [Test, Order(2)]
    public async Task GetPosts_ShouldReturnPostsAndOk()
    {
        //Arrange

        //Act
        var response = await httpClient.GetAsync("/api/dataPost");
        List<PostTestModel>? postTestModels = await response.Content.ReadFromJsonAsync<List<PostTestModel>>();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        postTestModels.Should().NotBeNull();
        postTestModels.Should().HaveCount(count => count >= 1);
    }

    [Test, Order(3)]
    public async Task GetPost_ShouldReturnNotFound()
    {
        //Arrange
        string bogusGuid = "bogusGuid";

        //Act
        var response = await httpClient.GetAsync($"/api/dataPost/{bogusGuid}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test, Order(4)]
    public async Task GetPost_ShouldReturnPostAndOk()
    {
        //Arrange
        var setupResponse = await httpClient.GetAsync("/api/dataPost");
        List<PostTestModel>? postTestModels = await setupResponse.Content.ReadFromJsonAsync<List<PostTestModel>>();
        string? firstPostGuid = postTestModels!.FirstOrDefault()!.Guid;

        //Act
        var response = await httpClient.GetAsync($"/api/dataPost/{firstPostGuid}");
        PostTestModel? postTestModel = await response.Content.ReadFromJsonAsync<PostTestModel>();

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        postTestModel.Should().NotBeNull();
        postTestModel!.Guid.Should().Be(firstPostGuid);
    }

    [Test, Order(5)]
    public async Task EditPost_ShouldReturnBadRequest()
    {
        //Arrange
        var bogusEditPostModel = new Dictionary<string, string>();
        bogusEditPostModel.TryAdd("Guid", "bogusGuid");
        bogusEditPostModel.TryAdd("UserId", "userTestId");
        bogusEditPostModel.TryAdd("Title", "Test Title");
        bogusEditPostModel.TryAdd("Content", "test content");

        //Act
        var response = await httpClient.PutAsJsonAsync("/api/dataPost/", bogusEditPostModel);
        
        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test, Order(6)]
    public async Task EditPost_ShouldEditPostAndReturnOk()
    {
        //Arrange
        var setupResponse = await httpClient.GetAsync("/api/dataPost");
        List<PostTestModel>? postTestModels = await setupResponse.Content.ReadFromJsonAsync<List<PostTestModel>>();
        string? firstPostGuid = postTestModels!.FirstOrDefault()!.Guid;

        var editPostModel = new Dictionary<string, string>();
        editPostModel.TryAdd("Guid", firstPostGuid!);
        editPostModel.TryAdd("UserId", "userTestId");
        editPostModel.TryAdd("Title", "Test Title Edited");
        editPostModel.TryAdd("Content", "test content edited");

        //Act
        var response = await httpClient.PutAsJsonAsync("/api/dataPost/", editPostModel);

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test, Order(7)]
    public async Task DeletePost_ShouldReturnBadRequest()
    {
        //Arrange
        string bogusGuid = "bogusGuid";

        //Act
        var response = await httpClient.DeleteAsync($"/api/dataPost/{bogusGuid}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test, Order(8)]
    public async Task DeletePost_ShouldDeletePostAndReturnNoContent()
    {
        //Arrange
        var setupResponse = await httpClient.GetAsync("/api/dataPost");
        List<PostTestModel>? postTestModels = await setupResponse.Content.ReadFromJsonAsync<List<PostTestModel>>();
        string? firstPostGuid = postTestModels!.FirstOrDefault()!.Guid;

        //Act
        var response = await httpClient.DeleteAsync($"/api/dataPost/{firstPostGuid}");

        //Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        if (Environment.GetEnvironmentVariable("DataDatabaseInUse") == "SqlServer")
            ResetDatabaseHelperMethods.ResetSqlPostDatabase();
        else
            await ResetDatabaseHelperMethods.ResetNoSqlPostDatabase();
    }
}
