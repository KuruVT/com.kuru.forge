using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Forge.Utilities;

namespace Forge.Modules
{
    public static class FONTS
    {
        private const string LoggerSource = "FONTS Module";

        /// <summary>
        /// Removes all TTF and TTC font files from the specified directory and its subdirectories.
        /// </summary>
        /// <param name="directoryPath">The root directory path where TTF and TTC files will be searched and deleted.</param>
        /// <remarks>
        /// This method uses parallel processing with a maximum of 4 concurrent tasks to enhance performance.
        /// After deletion, a log entry is created indicating the number of files removed.
        /// </remarks>
        public static void RemoveAllFonts(string directoryPath)
        {
            var fontFiles = Directory.EnumerateFiles(directoryPath, "*.*", SearchOption.AllDirectories)
                                      .Where(file => file.EndsWith(".ttf") || file.EndsWith(".ttc"))
                                      .ToArray();
            int fileCount = fontFiles.Length;

            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 4 };

            Parallel.ForEach(fontFiles, parallelOptions, File.Delete);

            Logger.Log(Logger.LogType.Info, LoggerSource, $"Font cleanup completed. {fileCount} files removed.");
        }
    }
}