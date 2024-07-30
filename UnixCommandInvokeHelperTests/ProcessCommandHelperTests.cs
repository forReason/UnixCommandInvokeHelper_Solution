using System.Runtime.InteropServices;
using UnixCommandInvokeHelper;

namespace UnixCommandInvokeHelperTests;

public class ProcessCommandHelperTests
{
    [Fact]
    public async Task ListDirectoryContents_ShouldWorkCrossPlatform()
    {
        // Arrange
        var processCommandHelper = new ProcessCommandHelper();
        string commandText;
            
        // Use a directory that exists on both Windows (via WSL) and Unix systems
        string directoryPath = "/tmp";

        // Act
        var result = await processCommandHelper.ExecuteAsync($"ls {directoryPath}");

        // Assert
        Assert.False(string.IsNullOrEmpty(result.Output), "The command output should not be empty.");
        Assert.True(result.Errors == string.Empty, $"The command should not produce errors. Errors: {result.Errors}");
    }
    [Fact]
    public async Task ListDirectoryContents_ShouldWorkWindows()
    {
        // Arrange
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var processCommandHelper = new ProcessCommandHelper();
            var result = await processCommandHelper.ExecuteAsync($"ls /mnt/c/temp");

            // Assert
            Assert.False(string.IsNullOrEmpty(result.Output), "The command output should not be empty.");
            Assert.True(result.Errors == string.Empty, $"The command should not produce errors. Errors: {result.Errors}");
        }
    }
    [Fact]
    public async Task ListDirectoryContents_ShouldWorkWithWorkingDirOnWindows()
    {
        // Use a directory that exists on both Windows (via WSL) and Unix systems
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var processCommandHelper = new ProcessCommandHelper();
            DirectoryInfo testdir = new DirectoryInfo(@"C:\temp");
            var result = await processCommandHelper.ExecuteAsync("ls", testdir);

            // Assert
            Assert.False(string.IsNullOrEmpty(result.Output), "The command output should not be empty.");
            Assert.True(result.Errors == string.Empty, $"The command should not produce errors. Errors: {result.Errors}");
        }
    }
}