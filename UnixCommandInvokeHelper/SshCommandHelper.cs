

using Renci.SshNet;

namespace UnixCommandInvokeHelper
{
    public class SshCommandHelper
    {
        /// <summary>
        /// creates a new SshInstance wrapper for SshClient
        /// </summary>
        /// <remarks>
        /// executes each command in a new connection to ensure isolation between Commands
        /// </remarks>
        /// <param name="username">the username to use</param>
        /// <param name="password">the password to use (optional if keyFile is used)</param>
        /// <param name="keyFile">the keyfile for authentification (optional with password)</param>
        /// <param name="host">the target host</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public SshCommandHelper(string username, string? password = null, FileInfo? keyFile = null, string host = "localhost")
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException(nameof(username), "Username cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException(nameof(host), "Host cannot be null or empty.");
            }

            // Handling key file and password scenarios
            if (keyFile == null)
            {
                if (string.IsNullOrEmpty(password))
                {
                    throw new ArgumentNullException(nameof(password), "Password cannot be null or empty when key file is not provided.");
                }

                ConnectionInfo = new ConnectionInfo(host, username, new PasswordAuthenticationMethod(username, password));
            }
            else
            {
                if (!keyFile.Exists)
                {
                    throw new FileNotFoundException("The specified key file does not exist.", keyFile.FullName);
                }

                PrivateKeyFile pkFile = string.IsNullOrEmpty(password) ? new PrivateKeyFile(keyFile.FullName) : new PrivateKeyFile(keyFile.FullName, password);
                ConnectionInfo = new ConnectionInfo(host, username, new PrivateKeyAuthenticationMethod(username, pkFile));
            }
        }

        private ConnectionInfo ConnectionInfo;

        /// <summary>
        /// runs a command against the ssh connection, returning the result
        /// </summary>
        /// <param name="command">the command string to execute</param>
        /// <returns></returns>
        public CommandResult RunCommand(string command)
        {
            CommandResult result = new CommandResult();
            result.StartTime = DateTime.Now;
            try
            {
                using (var client = new SshClient(ConnectionInfo))
                {
                    client.Connect();
                    SshCommand executionResult = client.RunCommand(command);
                    result.Errors = executionResult.Error;
                    result.Output = executionResult.Result;
                }
            }
            catch (Exception Ex)
            {
                result.Exception = Ex;
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
        /// <warning>
        /// Using sudo in scripts and applications can pose significant security risks, especially when passwords are involved.
        /// Only use this method in trusted and secure environments.
        /// </warning>
        public CommandResult RunCommandSudo(string command, string password = "")
        {
            if (!string.IsNullOrEmpty(password))
            {
                return RunCommand($"echo '{password}' | sudo -S {command}");
            }
            else
            {
                return RunCommand($"sudo {command}");
            }
        }
    }
}
