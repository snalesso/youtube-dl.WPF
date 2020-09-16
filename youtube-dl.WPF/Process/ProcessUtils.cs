using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using youtube_dl.WPF.Core;

namespace youtube_dl.WPF.Process
{
    public static class ProcessUtils
    {
        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="processStartInfo">The <see cref="T:System.Diagnostics.ProcessStartInfo" /> that contains the information that is used to start the process, including the file name and any command-line arguments.</param>
        /// <param name="standardOutput">List that lines written to standard output by the process will be added to</param>
        /// <param name="standardError">List that lines written to standard error by the process will be added to</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public static async Task<ProcessResults> RunAsync(
            ProcessStartInfo processStartInfo,
            List<string> standardOutput,
            List<string> standardError,
            CancellationToken cancellationToken)
        {
            // force some settings in the start info so we can capture the output
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            var tcs = new TaskCompletionSource<ProcessResults>();

            var process = new System.Diagnostics.Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
            };

            var standardOutputResults = new TaskCompletionSource<string[]>();
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                    standardOutput.Add(args.Data);
                else
                    standardOutputResults.SetResult(standardOutput.ToArray());
            };

            var standardErrorResults = new TaskCompletionSource<string[]>();
            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                    standardError.Add(args.Data);
                else
                    standardErrorResults.SetResult(standardError.ToArray());
            };

            var processStartTime = new TaskCompletionSource<DateTime>();

            process.Exited += async (sender, args) =>
            {
                // Since the Exited event can happen asynchronously to the output and error events, 
                // we await the task results for stdout/stderr to ensure they both closed.  We must await
                // the stdout/stderr tasks instead of just accessing the Result property due to behavior on MacOS.  
                // For more details, see the PR at https://github.com/jamesmanning/RunProcessAsTask/pull/16/
                tcs.TrySetResult(
                    new ProcessResults(
                        process,
                        await processStartTime.Task.ConfigureAwait(false),
                        await standardOutputResults.Task.ConfigureAwait(false),
                        await standardErrorResults.Task.ConfigureAwait(false)
                    )
                );
            };

            using (cancellationToken.Register(
                () =>
                {
                    tcs.TrySetCanceled();
                    try
                    {
                        if (!process.HasExited)
                            process.Kill();
                    }
                    catch (InvalidOperationException) { }
                }))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var startTime = DateTime.Now;
                if (process.Start() == false)
                {
                    tcs.TrySetException(new InvalidOperationException("Failed to start process"));
                }
                else
                {
                    try
                    {
                        startTime = process.StartTime;
                    }
                    catch (Exception)
                    {
                        // best effort to try and get a more accurate start time, but if we fail to access StartTime
                        // (for instance, process has already existed), we still have a valid value to use.
                    }
                    processStartTime.SetResult(startTime);

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }

                return await tcs.Task.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="processStartInfo">The <see cref="T:System.Diagnostics.ProcessStartInfo" /> that contains the information that is used to start the process, including the file name and any command-line arguments.</param>
        /// <param name="standardOutput">List that lines written to standard output by the process will be added to</param>
        /// <param name="standardError">List that lines written to standard error by the process will be added to</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// 
        public static async Task<int> RunAsync(
            ProcessStartInfo processStartInfo,
            Action<string> standardOutputHandler,
            Action<string> standardErrorHandler,
            Action<int> exitHandler,
            CancellationToken cancellationToken)
        {
            // force some settings in the start info so we can capture the output
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            var tcs = new TaskCompletionSource<int>();

            var process = new System.Diagnostics.Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true
                //PriorityClass = ProcessPriorityClass.Normal,
            };

            var standardOutputResults = new TaskCompletionSource<string[]>();
            process.OutputDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    standardOutputHandler(args.Data);
#if DEBUG
                    Console.WriteLine($"{YouTubeDL.OfficialName} - Output: " + args.Data);
#endif
                }
                else
                {
                    standardOutputResults.SetResult(null);
                }
            };

            var standardErrorResults = new TaskCompletionSource<string[]>();
            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    standardErrorHandler(args.Data);
#if DEBUG
                    Console.WriteLine($"{YouTubeDL.OfficialName} - Error: " + args.Data);
#endif
                }
                else
                {
                    standardErrorResults.SetResult(null);
                }
            };

            var processStartTime = new TaskCompletionSource<DateTime>();

            process.Exited += async (sender, args) =>
            {
                exitHandler(process.ExitCode);

                // Since the Exited event can happen asynchronously to the output and error events, 
                // we await the task results for stdout/stderr to ensure they both closed.  We must await
                // the stdout/stderr tasks instead of just accessing the Result property due to behavior on MacOS.  
                // For more details, see the PR at https://github.com/jamesmanning/RunProcessAsTask/pull/16/

                await processStartTime.Task.ConfigureAwait(false);
                await standardOutputResults.Task.ConfigureAwait(false);
                await standardErrorResults.Task.ConfigureAwait(false);

                tcs.TrySetResult(process.ExitCode);

                process.Dispose();
            };

            using (cancellationToken.Register(
                () =>
                {
                    tcs.TrySetCanceled();
                    try
                    {
                        if (!process.HasExited)
                            process.Kill();
                    }
                    catch (InvalidOperationException) { }
                }))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var startTime = DateTime.Now;
                if (process.Start() == false)
                {
                    tcs.TrySetException(new InvalidOperationException("Failed to start process"));
                }
                else
                {
                    try
                    {
                        // TODO: remove?
                        startTime = process.StartTime;
                    }
                    catch (Exception)
                    {
                        // best effort to try and get a more accurate start time, but if we fail to access StartTime
                        // (for instance, process has already existed), we still have a valid value to use.
                    }

                    processStartTime.SetResult(startTime);

                    // TODO: begin before process.Start & stop if start fails?
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                }

                return await tcs.Task.ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="fileName">An application or document which starts the process.</param>
        public static Task<ProcessResults> RunAsync(string fileName) => RunAsync(new ProcessStartInfo(fileName));

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="fileName">An application or document which starts the process.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public static Task<ProcessResults> RunAsync(string fileName, CancellationToken cancellationToken) => RunAsync(new ProcessStartInfo(fileName), cancellationToken);

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="fileName">An application or document which starts the process.</param>
        /// <param name="arguments">Command-line arguments to pass to the application when the process starts.</param>
        public static Task<ProcessResults> RunAsync(string fileName, string arguments) => RunAsync(new ProcessStartInfo(fileName, arguments));

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="fileName">An application or document which starts the process.</param>
        /// <param name="arguments">Command-line arguments to pass to the application when the process starts.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public static Task<ProcessResults> RunAsync(string fileName, string arguments, CancellationToken cancellationToken) => RunAsync(new ProcessStartInfo(fileName, arguments), cancellationToken);

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="processStartInfo">The <see cref="T:System.Diagnostics.ProcessStartInfo" /> that contains the information that is used to start the process, including the file name and any command-line arguments.</param>
        public static Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo) => RunAsync(processStartInfo, CancellationToken.None);

        /// <summary>
        /// Runs asynchronous process.
        /// </summary>
        /// <param name="processStartInfo">The <see cref="T:System.Diagnostics.ProcessStartInfo" /> that contains the information that is used to start the process, including the file name and any command-line arguments.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        public static Task<ProcessResults> RunAsync(ProcessStartInfo processStartInfo, CancellationToken cancellationToken) => RunAsync(processStartInfo, new List<string>(), new List<string>(), cancellationToken);
    }
}
