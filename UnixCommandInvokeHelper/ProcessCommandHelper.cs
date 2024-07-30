using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UnixCommandInvokeHelper
{
    /// <summary>
    /// Creates a Command which can be executed as a process.
    /// </summary>
    /// <remarks>
    /// The Result can be accessed with command.Errors & command.Output
    /// </remarks>
    public class ProcessCommandHelper
    {
        /// <summary>
        /// Converts a Windows path to a WSL path.
        /// </summary>
        /// <param name="windowsPath">The Windows path to convert</param>
        /// <returns>The converted WSL path</returns>
        public string ConvertPathToWsl(string windowsPath)
        {
            return "/mnt/" + windowsPath.Substring(0, 1).ToLower() + windowsPath.Substring(2).Replace("\\", "/");
        }
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <remarks>
        /// After the task is finished, you can access the results with command.Errors and command.Output.
        /// </remarks>
        /// <returns>Awaitable Task</returns>
        /// <param name="commandText">The command to execute</param>
        /// <param name="workingDirectory">The directory path to execute the command in (optional)</param>
        public async Task<CommandResult> ExecuteAsync(string commandText, DirectoryInfo? workingDirectory = null)
        {
            CommandResult result = new CommandResult();
            result.StartTime = DateTime.Now;

            using (var process = new Process())
            {
                try
                {
                    string escapedArgs = commandText.Replace("\"", "\\\"");
                    var processStartInfo = new ProcessStartInfo()
                    {
                        WindowStyle = ProcessWindowStyle.Hidden,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    };

                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        processStartInfo.FileName = "wsl.exe";
                        string commandstring = "";
                        if (workingDirectory != null) commandstring = $"cd {ConvertPathToWsl(workingDirectory.FullName)} &&";
                        commandstring += escapedArgs;
                        processStartInfo.Arguments = $"-e bash -c \"{commandstring}\"";
                    }
                    else
                    {
                        processStartInfo.FileName = "/bin/bash";
                        processStartInfo.Arguments = $"-c \"{escapedArgs}\"";
                        if (workingDirectory != null)
                        {
                            processStartInfo.WorkingDirectory = workingDirectory.FullName;
                        }
                    }
                        

                    process.StartInfo = processStartInfo;
                    process.Start();

                    result.Errors = await process.StandardError.ReadToEndAsync();
                    result.Output = await process.StandardOutput.ReadToEndAsync();
                    await process.WaitForExitAsync();
                }
                catch (Exception ex)
                {
                    result.Errors += ex.Message;
                }
            }

            result.EndTime = DateTime.Now;
            return result;
        }

        /// <summary>
        /// Executes the command with sudo privileges.
        /// </summary>
        /// <param name="password">The password for sudo. Leave empty if passwordless sudo is configured.</param>
        /// <remarks>
        /// <para>This method should be used with caution due to the security implications of handling sudo passwords.</para>
        /// <para>Ensure that the sudoers file is properly configured to minimize security risks.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// var command = new Command("apt-get update", "/tmp");
        /// await command.ExecuteSudoAsync("yourPassword");
        /// Console.WriteLine(command.Output);
        /// </code>
        /// </example>
        /// <warning>
        /// Using sudo in scripts and applications can pose significant security risks, especially when passwords are involved.
        /// Only use this method in trusted and secure environments.
        /// </warning>
        /// <param name="commandText">The command to execute (you do not have to prefix sudo)</param>
        /// <param name="password">The password to use</param>
        /// <param name="workingDirectory">The directory to execute the command in</param>
        /// <returns>Awaitable Task</returns>
        public async Task<CommandResult> ExecuteSudoAsync(string commandText, string password = "", DirectoryInfo? workingDirectory = null)
        {
            if (!string.IsNullOrEmpty(password))
            {
                commandText = $"echo {password} | sudo -S {commandText}";
            }
            else
            {
                commandText = $"sudo {commandText}";
            }
            return await ExecuteAsync(commandText, workingDirectory);
        }
    }
}
