using MVCAndWebAPIAuthAndAuthTest.EETestLibrary.HelperServices;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.Tests;

[SetUpFixture]
public class GlobalSetUpFixture
{
    private IProcessManagementService _processManagementService;

    [OneTimeSetUp]
    public void OnTimeSetupUp()
    {
        _processManagementService = new ProcessManagementService();
        _processManagementService.BuildAndRunApplication();

        Thread.Sleep(6000);
    }

    [OneTimeTearDown]
    public void OnTimeTearDown()
    {
        _processManagementService.TerminateApplication();
    }
}
