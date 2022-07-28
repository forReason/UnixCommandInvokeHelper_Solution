using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnixCommandInvokeHelper
{
    public class Command
    {
        public Command(string commandString, string workingDirectory = "~")
        { 
            CommandText = commandString;
            WorkingDirectory = workingDirectory;
        }
        public string CommandText { get; set; }
        public string WorkingDirectory { get; set; }
        public string Errors { get; private set; }
        public string Output { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public async void Execute()
        {
            StartTime = DateTime.Now;
            Errors = "";
            Output = "";

            var process = new Process();
            try
            {
                string escapedArgs = CommandText.Replace("\"", "\\\"");
                var processStartInfo = new ProcessStartInfo()
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = $"/bin/bash",
                    WorkingDirectory = this.WorkingDirectory,
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                process.StartInfo = processStartInfo;
                process.Start();
                Errors = process.StandardError.ReadToEnd();
                Output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                
            }
            catch (Exception ex)
            {
                Errors = ex.Message;
            }
            process.Close();
            EndTime = DateTime.Now;
        }
    }
}
