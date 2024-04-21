namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.HelperServices;

public class EmailReaderService
{
    public static string? GetLastEmailLink()
    {
        string directoryPath = @"C:\ProgramData\Changemaker Studios\Papercut SMTP\Incoming";

        // Get all .eml files in the directory
        List<string> emlFiles = Directory.GetFiles(directoryPath, "*.eml")
                                .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                                .ToList();

        //get the last one
        string? lastEmailFile = emlFiles.FirstOrDefault();
        if (lastEmailFile is null)
            return null!;

        //the last word is the email
        string emailContent = File.ReadAllText(lastEmailFile);
        string[] wordsOfLastEmail = emailContent.Split(" ");

        //get the last word that is the link and remove email formatting
        string link = wordsOfLastEmail.Last().Replace("=\r\n", "").Replace("\r\n", "");
        link = link.Replace("=3D", "=");

        //delete the file now that we have the link
        File.Delete(lastEmailFile);
        return link;
    }

    public static void DeleteLastEmailLink()
    {
        string directoryPath = @"C:\ProgramData\Changemaker Studios\Papercut SMTP\Incoming";

        List<string> emlFiles = Directory.GetFiles(directoryPath, "*.eml")
                                .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                                .ToList();

        //get the last one
        string? lastEmailFile = emlFiles.FirstOrDefault();
        if (lastEmailFile is not null)
            File.Delete(lastEmailFile);
    }
}
