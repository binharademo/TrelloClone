using System;
using System.IO;
using System.Diagnostics;

public class DiagnoseErrors
{
    public static void RunDiagnostics()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "build",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string output = process.StandardOutput.ReadToEnd();
        string error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        File.WriteAllText("build_output.txt", output);
        File.WriteAllText("build_error.txt", error);
        
        Console.WriteLine("Build output and errors have been saved to files.");
    }
}
