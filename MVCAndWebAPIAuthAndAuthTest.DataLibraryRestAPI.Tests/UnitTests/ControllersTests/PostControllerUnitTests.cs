using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Logic;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.RequestModels;
using MVCAndWebAPIAuthAndAuthTest.DataLibrary.Models.ResponseModels;
using MVCAndWebAPIAuthAndAuthTest.DataLibraryRestAPI.Controllers;
using MVCAndWebAPIAuthAndAuthTest.DataLibraryRestAPI.Tests.UnitTests.HelperMethods;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using System.Net;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibraryRestAPI.Tests.UnitTests.ControllersTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("konstantinos", "kinnaskonstantinos0@gmail.com")]
//TODO add the thing here
public class PostControllerUnitTests
{
    private IPostDataAccess _postDataAccess;
    private DataPostController _dataPostController; 

    [SetUp]
    public void Setup()
    {
        _postDataAccess = Substitute.For<IPostDataAccess>();
        _dataPostController = new DataPostController(_postDataAccess);
    }

    [Test]
    public async Task GetPosts_ShouldReturnInternalServerError()
    {
        //Arrange
        _postDataAccess.GetPostsAsync(30).Throws(new Exception());

        //Act
        var result = await _dataPostController.GetPosts();

        //Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();

        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task GetPosts_ShouldReturnPostsAndOk()
    {
        //Arrange
        _postDataAccess.GetPostsAsync(30).Returns(new List<PostResponseModel>()
        {
            new PostResponseModel(){ Guid = "1", UserId = "userId1"},
            new PostResponseModel(){ Guid = "2", UserId = "userId2"},
            new PostResponseModel(){ Guid = "3", UserId = "userId3"}
        });

        //Act
        var result = await _dataPostController.GetPosts();

        //Assert
        result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        var okObjectResult = result as OkObjectResult;
        var postResponseModels = okObjectResult!.Value as List<PostResponseModel>;
        postResponseModels.Should().NotBeNull().And.HaveCount(3);
    }

    [Test]
    public async Task GetPost_ShouldReturnInternalServerError()
    {
        //Arrange
        string postGuid = "1";
        _postDataAccess.GetPostAsync(postGuid).Throws(new Exception());

        //Act
        var result = await _dataPostController.GetPost(postGuid);

        //Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();

        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task GetPost_ShouldReturnNotFound()
    {
        //Arrange
        string postGuid = "bogusPostGuid";
        _postDataAccess.GetPostAsync(postGuid).ReturnsNull();

        //Act
        var result = await _dataPostController.GetPost(postGuid);

        //Assert
        result.Should().NotBeNull().And.BeOfType<NotFoundResult>();

        var notFoundObjectResult = result as NotFoundResult;
        notFoundObjectResult!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetPost_ShouldReturnPostAndOk()
    {
        //Arrange
        string postGuid = "validPostGuid";
        _postDataAccess.GetPostAsync(postGuid).Returns(new PostResponseModel() { Guid = postGuid});

        //Act
        var result = await _dataPostController.GetPost(postGuid);

        //Assert
        result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        var okObjectResult = result as OkObjectResult;
        okObjectResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        PostResponseModel? postResponseModel = okObjectResult.Value as PostResponseModel;
        postResponseModel.Should().NotBeNull();
        postResponseModel!.Guid.Should().Be(postGuid);
    }

    [Test]
    public async Task AddPost_ShouldReturnInternalServerError()
    {
        //Arrange
        CreatePostRequestModel createPostRequestModel = new CreatePostRequestModel();
        _postDataAccess.CreatePostAsync(createPostRequestModel).Throws(new Exception());

        //Act
        var result = await _dataPostController.AddPost(createPostRequestModel);

        //Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();

        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task AddPost_ShouldReturnBadRequest()
    {
        //Arrange
        CreatePostRequestModel createPostRequestModel = new CreatePostRequestModel();
        _postDataAccess.CreatePostAsync(createPostRequestModel).ReturnsNull();

        //Act
        var result = await _dataPostController.AddPost(createPostRequestModel);

        //Assert
        result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

        var objectResult = result as BadRequestObjectResult;
        objectResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task AddPost_ShouldReturnPostAndCreated()
    {
        //Arrange
        string createdPostGuid = "createdPostGuid1";
        CreatePostRequestModel createPostRequestModel = new CreatePostRequestModel() { 
            Content = "post content", Title = "Post Title", UserId = "userId1"
        };
        _postDataAccess.CreatePostAsync(createPostRequestModel).Returns(createdPostGuid);

        //Act
        var result = await _dataPostController.AddPost(createPostRequestModel);

        //Assert
        result.Should().NotBeNull().And.BeOfType<CreatedAtActionResult>();

        var objectResult = result as CreatedAtActionResult;
        objectResult!.StatusCode.Should().Be((int)HttpStatusCode.Created);

        var createdAtActionObjectResult = result as CreatedAtActionResult;
        PostResponseModel? returnedPostResponseModel = createdAtActionObjectResult!.Value as PostResponseModel;
        returnedPostResponseModel.Should().NotBeNull();
        returnedPostResponseModel!.Guid.Should().Be(createdPostGuid);

        createdAtActionObjectResult.ActionName.Should().Be(nameof(DataPostController.GetPost));
        createdAtActionObjectResult!.RouteValues!["guid"].Should().Be(createdPostGuid);
    }

    [Test]
    public async Task EditPost_ShouldReturnBadRequest_BecauseUserDoesNotOwnPost()
    {
        //Arrange
        string userId = "userWhoDoesNotOwnThePost_Id";
        _postDataAccess.GetPostsOfUserAsync(userId).ReturnsNull();
        EditPostRequestModel editPostRequestModel = new EditPostRequestModel()
        {
            Guid = "postGuid1",
            Content = "post content",
            Title = "Post Title",
            UserId = userId
        };

        //Act
        var result = await _dataPostController.EditPost(editPostRequestModel);

        //Assert
        result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

        var badRequestObjectResult = result as BadRequestObjectResult;
        badRequestObjectResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        string errorMessage = JsonHelperMethods.GetStringValueFromJsonObject(result);
        errorMessage.Should().Be("UserDoesNotOwnPost");
    }

    [Test]
    public async Task EditPost_ShouldReturnBadRequest_BecausePostUpdateFailed()
    {
        //Arrange
        string userId = "userWhoOwnsThePost_Id";
        string guid = "postGuid1";
        _postDataAccess.GetPostsOfUserAsync(userId).Returns(new List<PostResponseModel>()
        {
            new PostResponseModel() { Guid = guid, UserId = userId },
            new PostResponseModel(){ Guid = "2", UserId = userId },
            new PostResponseModel(){ Guid = "3", UserId = userId }
        });
        EditPostRequestModel editPostRequestModel = new EditPostRequestModel() { Guid = guid, UserId = userId };

        _postDataAccess.UpdatePostAsync(guid, editPostRequestModel).Returns(false);

        //Act
        var result = await _dataPostController.EditPost(editPostRequestModel);

        //Assert
        var badRequestObjectResult = result as BadRequestObjectResult;
        badRequestObjectResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

        string errorMessage = JsonHelperMethods.GetStringValueFromJsonObject(result);
        errorMessage.Should().Be("FailedPostUpdate");
    }

    [Test]
    public async Task EditPost_ShouldReturnOk()
    {
        //Arrange
        string userId = "userWhoOwnsThePost_Id";
        string guid = "postGuid1";
        var postToBeEdited = new EditPostRequestModel() { Guid = guid, UserId = userId };
        _postDataAccess.GetPostsOfUserAsync(userId).Returns(new List<PostResponseModel>()
        {
            new PostResponseModel() { Guid = guid, UserId = userId },
            new PostResponseModel(){ Guid = "2", UserId = userId },
            new PostResponseModel(){ Guid = "3", UserId = userId }
        });
        _postDataAccess.UpdatePostAsync(guid, postToBeEdited).Returns(true);

        //Act
        var result = await _dataPostController.EditPost(postToBeEdited);

        //Assert
        result.Should().NotBeNull().And.BeOfType<OkResult>();

        var okObjectResult = result as OkResult;
        okObjectResult!.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Test]
    public async Task DeletePost_ShouldReturnInternalServerError()
    {
        //Arrange
        string postGuid = "postGuid";
        _postDataAccess.DeletePostAsync(postGuid).Throws(new Exception());

        //Act
        var result = await _dataPostController.DeletePost(postGuid);

        //Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();

        var objectResult = result as ObjectResult;
        objectResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task DeletePost_ShouldReturnBadRequest()
    {
        //Arrange
        string postGuid = "postGuid";
        _postDataAccess.DeletePostAsync(postGuid).Returns(false);

        //Act
        var result = await _dataPostController.DeletePost(postGuid);

        //Assert
        //TODO add more assertions here
        result.Should().NotBeNull().And.BeOfType<BadRequestObjectResult>();

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task DeletePost_ShouldReturnNoContent()
    {
        //Arrange
        string postGuid = "postGuid";
        _postDataAccess.DeletePostAsync(postGuid).Returns(true);

        //Act
        var result = await _dataPostController.DeletePost(postGuid);

        //Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();

        var noContentResult = result as NoContentResult;
        noContentResult!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
    }


}
