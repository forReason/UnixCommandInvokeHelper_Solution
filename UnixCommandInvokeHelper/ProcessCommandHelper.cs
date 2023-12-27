using System.Diagnostics;

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
        /// executes the command.
        /// </summary>
        /// <remarks>
        /// after the task is finished, you can access the results with command.Errors and command.Output
        /// </remarks>
        /// <returns>awaitable Tak</returns>
        /// <param name="commandText">the command to execute</param>
        /// <param name="workingDirectory">the directory path to execute the command in (optional)</param>
        /// <returns></returns>
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
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{escapedArgs}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false
                    };
                    if (workingDirectory != null)
                        processStartInfo.WorkingDirectory = workingDirectory.FullName;
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
        /// <param name="commandText">the command to execute (you do not have to prefix sudo)</param>
        /// <param name="password">the password to use</param>
        /// <param name="workingDirectory">the directory to execute the command in</param>
        /// <returns></returns>
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
