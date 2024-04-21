namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.HelperServices
{
    public interface IProcessManagementService
    {
        void BuildAndRunApplication();
        void TerminateApplication();
    }
}