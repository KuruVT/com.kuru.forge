using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Forge.Modules;
using Forge.Utilities;

using UnityEditor;

namespace Forge.Decompiler
{
    public static class Decompiler
    {
        private static readonly DecompilationConfig Settings = DecompilationConfig.Load();
        private const string LoggerSource = "Decompiler";

        /// <summary>
        /// Decompiles a set of ERA files to the specified output directory.
        /// </summary>
        /// <param name="files">Array of ERA file paths to be decompiled.</param>
        /// <param name="outputPath">The directory where decompiled files will be saved.</param>
        public static async Task FromEra(string[] files, string outputPath)
        {
            Logger.Log(Logger.LogType.Info, LoggerSource, "Decompilation session started.");

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

            EditorPrefs.SetBool("kCompressAssetsOnImport", false);
            AssetDatabase.StartAssetEditing();

            Logger.Log(Logger.LogType.Info, LoggerSource, $"Processing {files.Length} ERA files.");
            TaskProgress.Initialize("Decompiling ERA's");

            // Determine the degree of parallelism
            int maxParallelism = Settings.EnableParallelProcessing
                ? (Settings.ParallelThreads > 0 ? Settings.ParallelThreads : Environment.ProcessorCount / 2)
                : 1;

            // Process files using FileProcessor
            var processingTasks = files.Select(file => ProcessFileAsync(file, outputPath, maxParallelism));
            await Task.WhenAll(processingTasks);

            FileProcessor.DeleteTempFolders(outputPath);

            if (Settings.CleanUpXmb)
                XMB.RemoveAll(outputPath);

            if (Settings.CleanUpEraDef)
                ERADEF.RemoveAll(outputPath);

            if (Settings.ConvertToDds)
                DDX.ConvertAll(outputPath);

            FONTS.RemoveAllFonts(outputPath);

            TaskProgress.Complete();
            Logger.Log(Logger.LogType.Info, LoggerSource, "Decompilation completed.");

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Processes a single ERA file asynchronously.
        /// </summary>
        /// <param name="file">The ERA file to process.</param>
        /// <param name="outputPath">The directory where processed files will be saved.</param>
        /// <param name="maxParallelism">Maximum number of concurrent PhxTool instances.</param>
        private static async Task ProcessFileAsync(string file, string outputPath, int maxParallelism)
        {
            await FileProcessor.ProcessEraFile(file, outputPath);
            string currentFileName = Path.GetFileName(file);
            TaskProgress.UpdateBatch(maxParallelism, currentFileName, 1);
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
                await FileProcessor.ProcessEraFile(files[i], outputPath);

                processedFilesCount++;

                string currentFileName = Path.GetFileName(files[i]);
                TaskProgress.UpdateBatch(files.Length, currentFileName, processedFilesCount);
            }
        }
    }


    public class CustomAssetImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.mipmapEnabled = false;
        }
    }

}