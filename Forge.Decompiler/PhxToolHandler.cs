using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Forge.Utilities;

namespace Forge.Decompiler
{
    public static class PhxToolHandler
    {
        private const string LoggerSource = "PhxTool";
        private static readonly string TempDirectory = Path.Combine(Path.GetTempPath(), "PhxTool");
        private static readonly string ToolPath = Path.Combine(TempDirectory, "PhxTool.exe");
        private static readonly SemaphoreSlim ExtractionLock = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim ExecutionLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Extracts the embedded PhxTool.exe resource to a temporary directory if it doesn't already exist.
        /// </summary>
        /// <returns>The file path to the extracted PhxTool.exe.</returns>
        private static async Task<string> ExtractPhxToolAsync()
        {
            if (File.Exists(ToolPath))
                return ToolPath;

            await ExtractionLock.WaitAsync();
            try
            {
                if (File.Exists(ToolPath)) // Double-check inside the lock
                    return ToolPath;

                Logger.Log(Logger.LogType.Info, LoggerSource, $"Extracting PhxTool.exe to {TempDirectory}");

                Directory.CreateDirectory(TempDirectory);
                var assembly = Assembly.GetExecutingAssembly();

                foreach (var resourceName in assembly.GetManifestResourceNames())
                {
                    if (resourceName.EndsWith(".exe") || resourceName.EndsWith(".dll"))
                    {
                        var fileName = string.Join(".", resourceName.Split('.').Skip(2));
                        var filePath = Path.Combine(TempDirectory, fileName);

                        Logger.Log(Logger.LogType.Info, LoggerSource, $"Extracting {resourceName} to {filePath}");

                        using var resourceStream = assembly.GetManifestResourceStream(resourceName);
                        if (resourceStream == null)
                        {
                            Logger.Log(Logger.LogType.Error, LoggerSource, $"Missing resource stream for {resourceName}");
                            continue;
                        }

                        using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                        await resourceStream.CopyToAsync(fileStream);
                    }
                }

                if (!File.Exists(ToolPath))
                {
                    Logger.Log(Logger.LogType.Error, LoggerSource, "PhxTool.exe extraction failed.");
                    return string.Empty;
                }

                Logger.Log(Logger.LogType.Info, LoggerSource, "Successfully extracted PhxTool.exe");
                return ToolPath;
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, LoggerSource, $"Resource extraction failed: {ex}");
                return string.Empty;
            }
            finally
            {
                ExtractionLock.Release();
            }
        }

        /// <summary>
        /// Runs a single instance of PhxTool.exe with the specified arguments.
        /// </summary>
        /// <param name="arguments">The command-line arguments to pass to PhxTool.exe.</param>
        public static async Task RunPhxToolAsync(string arguments)
        {
            await ExecutionLock.WaitAsync();
            try
            {
                var toolPath = await ExtractPhxToolAsync();
                if (string.IsNullOrEmpty(toolPath))
                {
                    Logger.Log(Logger.LogType.Error, LoggerSource, "PhxTool.exe is missing. Aborting execution.");
                    return;
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = toolPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = TempDirectory
                };

                using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
                var tcs = new TaskCompletionSource<bool>();

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Logger.Log(Logger.LogType.Info, LoggerSource, $"[PhxTool] {e.Data}");
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        Logger.Log(Logger.LogType.Error, LoggerSource, $"[PhxTool] {e.Data}");
                };

                process.Exited += (sender, e) =>
                {
                    Logger.Log(Logger.LogType.Info, LoggerSource, $"PhxTool exited with code {process.ExitCode}");
                    tcs.SetResult(process.ExitCode == 0);
                };

                if (!process.Start())
                {
                    Logger.Log(Logger.LogType.Error, LoggerSource, "Failed to start PhxTool process.");
                    return;
                }

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await tcs.Task;
            }
            catch (Exception ex)
            {
                Logger.Log(Logger.LogType.Error, LoggerSource, $"Exception while running PhxTool: {ex}");
            }
            finally
            {
                ExecutionLock.Release();
            }
        }

        /// <summary>
        /// Runs multiple instances of PhxTool.exe in parallel with a specified degree of concurrency.
        /// </summary>
        /// <param name="argumentsList">A list of command-line arguments for each PhxTool.exe instance.</param>
        /// <param name="maxDegreeOfParallelism">The maximum number of concurrent PhxTool.exe instances.</param>
        public static async Task RunMultiplePhxToolsAsync(string[] argumentsList, int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1)
                throw new ArgumentOutOfRangeException(nameof(maxDegreeOfParallelism), "Must be at least 1.");

            var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

            var tasks = argumentsList.Select(async args =>
            {
                await semaphore.WaitAsync();
                try
                {
                    await RunPhxToolAsync(args);
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
