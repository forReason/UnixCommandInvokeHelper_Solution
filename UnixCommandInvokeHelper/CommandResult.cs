namespace UnixCommandInvokeHelper
{
    public struct CommandResult
    {
        /// <summary>
        /// after the command has been executed, you may find errors here
        /// </summary>
        public string? Errors { get; set; }

        /// <summary>
        /// any exceptions which might have ocurred
        /// </summary>
        public Exception? Exception { get; set; }

        /// <summary>
        /// after the command has been executed, you may find command output here
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// when the command was started
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// when the command finished
        /// </summary>
        public DateTime EndTime { get; set; }
    }
}
