# UnixCommandInvokeHelper

UnixCommandInvokeHelper is a C# library designed to facilitate the execution of Unix commands both locally and over SSH. It provides a simple interface to run commands, retrieve their output, and handle errors.

## Installation

You can install UnixCommandInvokeHelper from NuGet:

https://www.nuget.org/packages/UnixCommandInvokeHelper/

```sh
Install-Package UnixCommandInvokeHelper
```


## Usage

### ProcessCommandHelper

The `ProcessCommandHelper` class is used to execute local commands. It provides methods to run commands as a normal process or with `sudo` privileges.

#### Running a Command

```csharp
using UnixCommandInvokeHelper;
using System;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var commandHelper = new ProcessCommandHelper();
        var result = await commandHelper.ExecuteAsync("ls -la", new DirectoryInfo("/tmp"));

        Console.WriteLine("Output:");
        Console.WriteLine(result.Output);

        if (!string.IsNullOrEmpty(result.Errors))
        {
            Console.WriteLine("Errors:");
            Console.WriteLine(result.Errors);
        }
    }
}
```

#### Running a Command with Sudo

```csharp
// This example assumes passwordless sudo or proper handling of the sudo password.
var sudoResult = await commandHelper.ExecuteSudoAsync("apt-get update", "yourPassword");

Console.WriteLine("Sudo Output:");
Console.WriteLine(sudoResult.Output);
```

### SshCommandHelper

The `SshCommandHelper` class facilitates the execution of commands over an SSH connection. It supports both password and key-based authentication.

#### Establishing an SSH Connection and Running a Command

```csharp
using UnixCommandInvokeHelper;
using Renci.SshNet;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        var sshHelper = new SshCommandHelper("username", "password", null, "host.com");
        var commandResult = sshHelper.RunCommand("ls -la");

        Console.WriteLine("SSH Command Output:");
        Console.WriteLine(commandResult.Output);

        if (!string.IsNullOrEmpty(commandResult.Errors))
        {
            Console.WriteLine("SSH Command Errors:");
            Console.WriteLine(commandResult.Errors);
        }
    }
}
```

#### Running a Command with Sudo over SSH

```csharp
// This example assumes passwordless sudo or proper handling of the sudo password.
var sudoResult = sshHelper.RunCommandSudo("apt-get update", "yourPassword");

Console.WriteLine("Sudo SSH Output:");
Console.WriteLine(sudoResult.Output);
```

## Configuring Sudoers for Passwordless Command Execution

To securely run specific commands without entering a password using `sudo`, you need to edit the `sudoers` file. This is a critical file for Linux system security, so you should proceed with caution.

### Steps to Edit the Sudoers File

1. **Open the Sudoers File**:

    Use the `visudo` command for safe editing. This command checks for syntax errors before saving, which helps prevent lockouts.

    ```bash
    sudo visudo
    ```

2. **Add a New Rule**:

    At the end of the file, add a line specifying the user, the host, and the command. Replace `username`, `hostname`, and `/path/to/command` with your actual username, hostname, and the full path of the command you want to run without a password.

    ```plaintext
    username hostname = NOPASSWD: /path/to/command
    ```

    Example: To allow user `john` to run `apt-get update` without a password on a machine named `myhost`, add:

    ```plaintext
    john myhost = NOPASSWD: /usr/bin/apt-get update
    ```

3. **Save and Exit**:

    Save the file and exit the editor. If you are using `visudo`, it will automatically check for syntax errors.

4. **Testing**:

    Test the configuration by running the specified command with `sudo` from the user account. It should not prompt for a password.

    ```bash
    sudo /path/to/command
    ```

### Important Notes

- Be very cautious with this configuration. Allowing commands to run as root without a password can pose a significant security risk, especially if the command can be exploited to gain unauthorized access or escalate privileges.
- Always use the full path of the command in the `sudoers` file to avoid potential security issues with path manipulation.
- It's recommended to only allow specific, well-understood commands that require elevated privileges.

## License

This library is licensed under [MIT License
