using System.Diagnostics;

namespace MVCAndWebAPIAuthAndAuthTest.EETestLibrary.HelperServices;

public class ProcessManagementService : IProcessManagementService
{
    private Process? mvcProcess;
    private Process? apiProcess;
    private Process? dataLibraryProcess;
    private Process? emailLibraryProcess;

    public void BuildAndRunApplication()
    {
        // Get the solution directory with this convoluted way
        string solutionDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.Parent!.Parent!.Parent!.FullName;
        string solutionPath = $"{solutionDirectory}\\MVCAndWebAPIAuthAndAuthTest Solution.sln";
        string MVCPath = $"{solutionDirectory}\\MVCAndWebAPIAuthAndAuthTest.MVC\\MVCAndWebAPIAuthAndAuthTest.MVC.csproj";
        string APIPath = $"{solutionDirectory}\\MVCAndWebAPIAuthAndAuthTest.API\\MVCAndWebAPIAuthAndAuthTest.API.csproj";
        string DataLibraryRestApiPath = $"{solutionDirectory}\\MVCAndWebAPIAuthAndAuthTest.DataLibraryRestAPI\\MVCAndWebAPIAuthAndAuthTest.DataLibraryRestAPI.csproj";
        string EmailLibraryRestApiPath = $"{solutionDirectory}\\MVCAndWebAPIAuthAndAuthTest.EmailLibraryRestAPI\\MVCAndWebAPIAuthAndAuthTest.EmailLibraryRestAPI.csproj";

        // Build the solution
        var buildProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"build \"{solutionPath}\" --configuration Debug",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        buildProcess.OutputDataReceived += (sender, args) => Console.WriteLine(args.Data); // Consume standard output
        buildProcess.Start();
        buildProcess.BeginOutputReadLine(); // Begin asynchronously reading the standard output
        buildProcess.WaitForExit();

        // Start the web application
        mvcProcess = Process.Start("dotnet", $"run --project \"{MVCPath}\" --urls=https://localhost:44304");
        apiProcess = Process.Start("dotnet", $"run --project \"{APIPath}\" --urls=https://localhost:7189");
        dataLibraryProcess = Process.Start("dotnet", $"run --project \"{DataLibraryRestApiPath}\" --urls=https://localhost:7267");
        emailLibraryProcess = Process.Start("dotnet", $"run --project \"{EmailLibraryRestApiPath}\" --urls=https://localhost:7113");
    }

    public void TerminateApplication()
    {

        var processes = Process.GetProcesses();

        // Kill any additional processes, which for some reason keep existing after you kill the started processes
        foreach (Process process in processes)
        {
            if (process.ProcessName == "MSBuild" || process.ProcessName == "msedgewebview2" ||
                process.ProcessName == "dotnet")
            {
                process.Kill();
            }
        }

        //kill the processes
        mvcProcess?.Kill(true);
        apiProcess?.Kill(true);
        dataLibraryProcess?.Kill(true);
        emailLibraryProcess?.Kill(true);

        mvcProcess = null;
        apiProcess = null;
        dataLibraryProcess = null;
        emailLibraryProcess = null;
    }
}
