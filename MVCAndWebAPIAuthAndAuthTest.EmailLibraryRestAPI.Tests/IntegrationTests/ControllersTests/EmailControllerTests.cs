using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using MVCAndWebAPIAuthAndAuthTest.EmailLibraryRestAPI.Tests.TestModels;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;
using MVCAndWebAPIAuthAndAuthTest.EmailLibraryRestAPI.Tests.HelperMethods;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MVCAndWebAPIAuthAndAuthTest.EmailLibraryRestAPI.Tests.IntegrationTests.ControllersTests;

[TestFixture]
[Category("Integration")]
public class EmailControllerTests
{
    private HttpClient httpClient;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var webApplicationFactory = new WebApplicationFactory<Program>();
        httpClient = webApplicationFactory.CreateClient();
    }

    [Test, Order(1)]
    [Author("konstantinos kinnas", "kinnaskonstantinos0@gmail.com")]
    public async Task SendEmailAndSaveEmailEntry_ShouldReturnInternalServerError()
    {
        // Arrange
        Dictionary<string, string> emailModel = new Dictionary<string, string>();
        emailModel.TryAdd("Receiver", "NotAValidEmail");
        emailModel.TryAdd("Title", "Test Title");
        emailModel.TryAdd("Message", "Test Message");

        // Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/emails", emailModel);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Test, Order(2)]
    [Author("konstantinos kinnas", "kinnaskonstantinos0@gmail.com")]
    public async Task SendEmailAndSaveEmailEntry_ShouldCreateEmailEntryAndReturnOk()
    {
        // Arrange
        Dictionary<string, string> emailModel = new Dictionary<string, string>();
        emailModel.TryAdd("Receiver", "test@gmail.com");
        emailModel.TryAdd("Title", "Test Title");
        emailModel.TryAdd("Message", "Test Message");

        // Act
        HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/emails", emailModel);
        string? responseBody = await response.Content.ReadAsStringAsync();
        var warningMessage = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBody);
        warningMessage!.TryGetValue("WarningMessage", out string? warningValue);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        warningValue.Should().NotBe("DatabaseEntryCreationFailure");
    }

    [Test, Order(3)]
    [Author("konstantinos kinnas", "kinnaskonstantinos0@gmail.com")]
    public async Task GetEmailEntries_ShouldReturnEntriesAndReturnOk()
    {
        // Arrange

        // Act
        var response = await httpClient.GetAsync("/api/emails");
        List<EmailTestModel>? emailTestModels = await response.Content.ReadFromJsonAsync<List<EmailTestModel>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        emailTestModels.Should().NotBeNull();
        emailTestModels.Should().HaveCount(count => count >= 1);
    }

    [Test, Order(4)]
    [Author("konstantinos kinnas", "kinnaskonstantinos0@gmail.com")]
    public async Task GetEmailEntry_ShouldReturnNotFound()
    {
        // Arrange

        // Act
        var response = await httpClient.GetAsync("/api/emails/BogusRecordId");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test, Order(5)]
    [Author("konstantinos kinnas", "kinnaskonstantinos0@gmail.com")]
    public async Task GetEmailEntry_ShouldReturnEntryAndReturnOk()
    {
        // Arrange
        var setupResponse = await httpClient.GetAsync("/api/emails");
        List<EmailTestModel>? emailTestModels = await setupResponse.Content.ReadFromJsonAsync<List<EmailTestModel>>();
        string? firstEmailTestModelId = emailTestModels!.FirstOrDefault()!.Id;

        // Act
        var response = await httpClient.GetAsync($"/api/emails/{firstEmailTestModelId}");
        EmailTestModel? emailTestModel = await response.Content.ReadFromJsonAsync<EmailTestModel>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        emailTestModel.Should().NotBeNull();
        emailTestModel!.Id.Should().Be(firstEmailTestModelId);
    }

    [Test, Order(6)]
    [Author("konstantinos kinnas", "kinnaskonstantinos0@gmail.com")]
    public async Task DeleteEmailEntry_ShouldReturnNotFound()
    {
        // Arrange
        
        // Act
        var response = await httpClient.DeleteAsync($"/api/emails/BogusRecordId");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test, Order(7)]
    [Author("konstantinos kinnas", "kinnaskonstantinos0@gmail.com")]
    public async Task DeleteEmailEntry_ShouldDeleteEntryAndReturnNoContent()
    {
        // Arrange
        var setupResponse = await httpClient.GetAsync("/api/emails");
        List<EmailTestModel>? emailTestModels = await setupResponse.Content.ReadFromJsonAsync<List<EmailTestModel>>();

        // Act
        var response = await httpClient.DeleteAsync($"/api/emails/{emailTestModels!.FirstOrDefault()!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {        
        EmailFileHelperMethods.DeleteAllEmailFiles();
        if (Environment.GetEnvironmentVariable("EmailDatabaseInUse") == "SqlServer")
            ResetDatabaseHelperMethods.ResetSqlEmailDatabase();
        else
            await ResetDatabaseHelperMethods.ResetNoSqlEmailDatabase();
    }
}
