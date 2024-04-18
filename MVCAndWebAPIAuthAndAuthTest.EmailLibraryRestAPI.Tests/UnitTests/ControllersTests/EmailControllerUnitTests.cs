using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MVCAndWebAPIAuthAndAuthTest.EmailLibrary;
using MVCAndWebAPIAuthAndAuthTest.EmailLibrary.DataAccessLogic;
using MVCAndWebAPIAuthAndAuthTest.EmailLibrary.Models.RequestModels;
using MVCAndWebAPIAuthAndAuthTest.EmailLibrary.Models.ResponseModels;
using MVCAndWebAPIAuthAndAuthTest.EmailLibraryRestAPI.Controllers;
using MVCAndWebAPIAuthAndAuthTest.EmailLibraryRestAPI.Tests.UnitTests.HelperMethods;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using System.Net;

namespace MVCAndWebAPIAuthAndAuthTest.EmailLibraryRestAPI.Tests.UnitTests.ControllersTests;

[TestFixture]
[Category("Unit")]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[Author("Konstantinos Kinnas", "kinnaskonstantinos0@gmail.com")]
public class EmailControllerUnitTests
{
    private EmailsController _emailsController;
    private IEmailDataAccess _emailDataAccess;
    private IEmailService _emailService;

    [SetUp]
    public void Setup()
    {
        _emailDataAccess = Substitute.For<IEmailDataAccess>();
        _emailService = Substitute.For<IEmailService>();
        _emailsController = new EmailsController(_emailDataAccess, _emailService);
    }

    [Test]
    public async Task GetEmailEntry_ShouldReturnInternalServerError()
    {
        //Arrange
        string bogusEmailEntryId = "bogusId";
        _emailDataAccess.GetEmailEntryAsync(bogusEmailEntryId).Throws(new Exception());

        //Act
        var result = await _emailsController.GetEmailEntry(bogusEmailEntryId);

        //Assert
        result.Should().NotBeNull()
              .And.BeOfType<ObjectResult>();

        var statusCodeResult = result as ObjectResult;
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task GetEmailEntry_ShouldReturnNotFound()
    {
        //Arrange
        string bogusEmailEntryId = "bogusId";
        _emailDataAccess.GetEmailEntryAsync(bogusEmailEntryId).ReturnsNull();
        
        //Act
        var result = await _emailsController.GetEmailEntry(bogusEmailEntryId);

        //Assert
        result.Should().NotBeNull()
              .And.BeOfType<NotFoundResult>();

        var notFoundResult = result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
    }

    [Test]
    public async Task GetEmailEntry_ShouldReturnEntryAndOk()
    {
        //Arrange
        string validEmailEntryId = "validId";
        _emailDataAccess.GetEmailEntryAsync(validEmailEntryId).Returns(new EmailResponseModel());

        //Act
        var result = await _emailsController.GetEmailEntry(validEmailEntryId);

        //Assert
        result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        var okObjectResult = result as OkObjectResult;
        okObjectResult.Should().NotBeNull();
    }

    [Test]
    public async Task GetEmailEntries_ShouldReturnInternalServerError()
    {
        //Arrange
        _emailDataAccess.GetEmailEntriesAsync().Throws(new Exception());

        //Act
        var result = await _emailsController.GetEmailEntries();

        //Assert
        result.Should().NotBeNull()
              .And.BeOfType<ObjectResult>();

        var statusCodeResult = result as ObjectResult;
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task GetEmailEntries_ShouldReturnEntriesAndOk()
    {
        //Arrange
        _emailDataAccess.GetEmailEntriesAsync().Returns(new List<EmailResponseModel>() { 
            new EmailResponseModel(){ Id = "1", Title="Title1", Message = "Message1", Receiver = "receiver1@gmail.com"},
            new EmailResponseModel(){ Id = "2", Title="Title2", Message = "Message2", Receiver = "receiver2@gmail.com"},
            new EmailResponseModel(){ Id = "3", Title="Title3", Message = "Message3", Receiver = "receiver2@gmail.com"}
        });

        //Act
        var result = await _emailsController.GetEmailEntries();

        //Assert
        result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        var derivedOkObjectResult = result as OkObjectResult;
        var returnedEmailResponseModels = derivedOkObjectResult!.Value as List<EmailResponseModel>;

        returnedEmailResponseModels.Should().NotBeNull();
        returnedEmailResponseModels.Should().HaveCount(3);
    }

    [Test]
    public async Task SendEmailAndSaveEmailEntry_ShouldReturnBadRequest()
    {
        //Arrange
        string receiver = "receiver@gmail.com";
        string title = "title";
        string bogusMessage = "message";
        EmailRequestModel emailRequestModel = new EmailRequestModel() { Receiver = receiver, Title = title, Message = bogusMessage };
        
        _emailService.SendEmailAsync(receiver, title, bogusMessage).Returns(false);

        //Act
        var result = await _emailsController.SendEmailAndSaveEmailEntry(emailRequestModel);

        //Assert
        result.Should().NotBeNull().And.BeOfType<BadRequestResult>();

        var returnedStatusCode = result as BadRequestResult;
        returnedStatusCode!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task SendEmailAndSaveEmailEntry_ShouldReturnOkAndWarningMessage()
    {
        //Arrange
        string receiver = "receiver@gmail.com";
        string title = "title";
        string bogusMessage = "message";
        EmailRequestModel emailRequestModel = new EmailRequestModel() { Receiver = receiver, Title = title, Message = bogusMessage };
        
        _emailService.SendEmailAsync(receiver, title, bogusMessage).Returns(true);
        _emailDataAccess.SaveEmailEntryAsync(emailRequestModel).ReturnsNull();

        //Act
        var result = await _emailsController.SendEmailAndSaveEmailEntry(emailRequestModel);

        //Assert
        result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        string warningMessage = JsonHelperMethods.GetStringValueFromJsonObject(result); 
        warningMessage.Should().NotBe("None");
    }

    [Test]
    public async Task SendEmailAndSaveEmailEntry_ShouldReturnOk()
    {
        //Arrange
        string receiver = "receiver@gmail.com";
        string title = "title";
        string bogusMessage = "message";
        EmailRequestModel emailRequestModel = new EmailRequestModel() { Receiver = receiver, Title = title, Message = bogusMessage };

        _emailService.SendEmailAsync(receiver, title, bogusMessage).Returns(true);
        _emailDataAccess.SaveEmailEntryAsync(emailRequestModel)!.Returns("NewlyCreatedEmailEntryId");

        //Act
        var result = await _emailsController.SendEmailAndSaveEmailEntry(emailRequestModel);

        //Assert
        result.Should().NotBeNull().And.BeOfType<OkObjectResult>();

        string warningMessage = JsonHelperMethods.GetStringValueFromJsonObject(result);
        warningMessage.Should().Be("None");
    }

    [Test]
    public async Task DeleteEmailEntry_ShouldReturnInternalServerError()
    {
        //Arrange
        string emailEntryId = "emailEntryId";
        _emailDataAccess.DeleteEmailEntryAsync(emailEntryId).Throws(new Exception());

        //Act
        var result = await _emailsController.DeleteEmailEntry(emailEntryId);

        //Assert
        result.Should().NotBeNull().And.BeOfType<ObjectResult>();

        var statusCodeResult = result as ObjectResult;
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
    }

    [Test]
    public async Task DeleteEmailEntry_ShouldReturnNotFound()
    {
        //Arrange
        string emailEntryId = "bogusEmailEntryId";
        _emailDataAccess.DeleteEmailEntryAsync(emailEntryId).Returns(false);

        //Act
        var result = await _emailsController.DeleteEmailEntry(emailEntryId);

        //Assert
        result.Should().NotBeNull().And.BeOfType<NotFoundObjectResult>();

        var statusCodeResult = result as NotFoundObjectResult;
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteEmailEntry_ShouldReturnNoContent()
    {
        //Arrange
        string emailEntryId = "existingEmailEntryId";
        _emailDataAccess.DeleteEmailEntryAsync(emailEntryId).Returns(true);

        //Act
        var result = await _emailsController.DeleteEmailEntry(emailEntryId);

        //Assert
        result.Should().NotBeNull().And.BeOfType<NoContentResult>();

        var statusCodeResult = result as NoContentResult;
        statusCodeResult!.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
    }

}
