namespace MVCAndWebAPIAuthAndAuthTest.EmailLibraryRestAPI.Tests.IntegrationTests.HelperMethods;

public class EmailFileHelperMethods
{
    public static void DeleteAllEmailFiles()
    {
        string directoryPath = @"C:\ProgramData\Changemaker Studios\Papercut SMTP\Incoming";

        // Get all .eml files in the directory
        List<string> emlFiles = Directory.GetFiles(directoryPath, "*.eml").ToList();

        foreach (string emlFile in emlFiles)
            File.Delete(emlFile);
    }
}
