using System.IO;
using System.Threading.Tasks;

using UnityEngine;

namespace Forge
{
    public static class FileProcessor
    {
        private const string LoggerSource = "FileProcessor";

        /// <summary>
        /// Processes a single ERA file by decrypting and expanding its contents.
        /// </summary>
        /// <param name="toolPath">Path to the PhxTool executable.</param>
        /// <param name="file">The ERA file to be processed.</param>
        /// <param name="outputPath">The directory where processed files will be saved.</param>
        public static async Task ProcessEraFile(string toolPath, string file, string outputPath)
        {
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
            string tempDir = Path.Combine(outputPath, "temp", fileNameWithoutExt);

            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            await PhxToolHandler.RunPhxTool(toolPath, BuildArguments("decrypt", fileNameWithoutExt, Path.GetDirectoryName(file), tempDir));

            string decryptedFile = Path.Combine(tempDir, fileNameWithoutExt + ".era.bin");
            if (!File.Exists(decryptedFile))
            {
                Directory.Delete(tempDir, true);
                return;
            }

            await PhxToolHandler.RunPhxTool(toolPath, BuildExpandArguments(fileNameWithoutExt, tempDir, outputPath));
        }

        /// <summary>
        /// Deletes temporary folders created during file processing.
        /// </summary>
        /// <param name="outputPath">The root output directory containing temporary folders.</param>
        public static void DeleteTempFolders(string outputPath)
        {
            string tempRoot = Path.Combine(outputPath, "temp");
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, true);
        }

        /// <summary>
        /// Builds command-line arguments for decrypting ERA files.
        /// </summary>
        /// <param name="mode">The mode of operation (e.g., decrypt).</param>
        /// <param name="fileName">The name of the file without its extension.</param>
        /// <param name="filePath">The path of the original file.</param>
        /// <param name="outputPath">The directory where the output will be saved.</param>
        /// <returns>A formatted string of command-line arguments.</returns>
        private static string BuildArguments(string mode, string fileName, string filePath, string outputPath)
        {
            return $"--env=phx --tool=era --mode={mode} --path=\"{filePath}\" --name={fileName} --out=\"{outputPath}\"";
        }

        /// <summary>
        /// Builds command-line arguments for expanding decrypted ERA files.
        /// </summary>
        /// <param name="fileNameWithoutExt">The name of the file without its extension.</param>
        /// <param name="tempDir">The temporary directory where the decrypted file is located.</param>
        /// <param name="outputPath">The directory where the expanded files will be saved.</param>
        /// <returns>A formatted string of command-line arguments.</returns>
        private static string BuildExpandArguments(string fileNameWithoutExt, string tempDir, string outputPath)
        {
            return $"--env=phx --tool=era --mode=expand --path=\"{Path.Combine(tempDir, fileNameWithoutExt)}\" --name={fileNameWithoutExt} --out=\"{outputPath}\"";
        }
    }
}
