using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Forge.Modules;
using Forge.Utilities;

using UnityEngine;

namespace Forge
{
    public static class Decompiler
    {
        private static readonly string ToolPath = PhxToolHandler.GetToolPath();
        private static readonly DecompilationConfig Settings = DecompilationConfig.LoadConfig();
        private const string LoggerSource = "Decompiler";

        /// <summary>
        /// Decompiles a set of ERA files to the specified output directory.
        /// </summary>
        /// <param name="files">Array of ERA file paths to be decompiled.</param>
        /// <param name="outputPath">The directory where decompiled files will be saved.</param>
        public static async Task FromEra(string[] files, string outputPath)
        {
            Logger.Log(Logger.LogType.Info, LoggerSource, "Decompilation session started.");

            if (string.IsNullOrEmpty(ToolPath) || !File.Exists(ToolPath))
            {
                Logger.Log(Logger.LogType.Error, LoggerSource, "Decompilation aborted: Tool path invalid or PhxTool.exe missing.");
                return;
            }

            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
                Logger.Log(Logger.LogType.Info, LoggerSource, "Output directory established.");
            }

            if (!DiskSpaceChecker.HasSufficientDiskSpace(files, outputPath))
            {
                Logger.Log(Logger.LogType.Error, LoggerSource, "Decompilation halted: Insufficient disk space.");
                TaskProgress.Complete();
                return;
            }

            Logger.Log(Logger.LogType.Info, LoggerSource, $"Processing {files.Length} ERA files.");
            TaskProgress.Initialize("Decompiling ERA's");

            if (Settings.EnableParallelProcessing)
                await ProcessFilesInParallel(files, outputPath);
            else
                await ProcessFilesSequentially(files, outputPath);

            FileProcessor.DeleteTempFolders(outputPath);

            if (Settings.CleanUpXmb)
                XMB.RemoveAll(outputPath);

            if (Settings.CleanUpEraDef)
                ERADEF.RemoveAll(outputPath);

            TaskProgress.Complete();
            Logger.Log(Logger.LogType.Info, LoggerSource, "Decompilation completed.");
        }

        /// <summary>
        /// Processes ERA files in parallel to improve performance.
        /// </summary>
        /// <param name="files">Array of ERA file paths to be processed.</param>
        /// <param name="outputPath">The directory where processed files will be saved.</param>
        private static Task ProcessFilesInParallel(string[] files, string outputPath)
        {
            int parallelThreads = Settings.ParallelThreads > 0 ? Settings.ParallelThreads : System.Environment.ProcessorCount / 2;
            int processedFilesCount = 0;

            return Task.Run(() =>
            {
                Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = parallelThreads }, (file) =>
                {
                    FileProcessor.ProcessEraFile(ToolPath, file, outputPath).GetAwaiter().GetResult();

                    int currentProcessedCount = Interlocked.Increment(ref processedFilesCount);

                    string currentFileName = Path.GetFileName(file);
                    TaskProgress.UpdateBatch(files.Length, currentFileName, currentProcessedCount);
                });
            });
        }

        /// <summary>
        /// Processes ERA files sequentially.
        /// </summary>
        /// <param name="files">Array of ERA file paths to be processed.</param>
        /// <param name="outputPath">The directory where processed files will be saved.</param>
        private static async Task ProcessFilesSequentially(string[] files, string outputPath)
        {
            int processedFilesCount = 0;

            for (int i = 0; i < files.Length; i++)
            {
                await FileProcessor.ProcessEraFile(ToolPath, files[i], outputPath);

                processedFilesCount++;

                string currentFileName = Path.GetFileName(files[i]);
                TaskProgress.UpdateBatch(files.Length, currentFileName, processedFilesCount);
            }
        }
    }
}