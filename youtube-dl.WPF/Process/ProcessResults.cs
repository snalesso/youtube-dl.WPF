using System;
using System.Diagnostics;

namespace youtube_dl.WPF.Process
{
    public sealed class ProcessResults : IDisposable
    {
        public ProcessResults(System.Diagnostics.Process process, DateTime processStartTime, string[] standardOutput, string[] standardError)
        {
            this.Process = process;
            this.ExitCode = process.ExitCode;
            this.RunTime = process.ExitTime - processStartTime;
            this.StandardOutput = standardOutput;
            this.StandardError = standardError;
        }

        public System.Diagnostics.Process Process { get; }
        public int ExitCode { get; }
        public TimeSpan RunTime { get; }
        public string[] StandardOutput { get; }
        public string[] StandardError { get; }
        public void Dispose() { this.Process.Dispose(); }
    }
}
