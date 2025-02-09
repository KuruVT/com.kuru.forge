using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Forge.Modules
{
    public static class XMB
    {
        private const string LoggerSource = "XMB Module";

        /// <summary>
        /// Removes all XMB files from the specified directory and its subdirectories.
        /// </summary>
        /// <param name="directoryPath">The root directory path where XMB files will be searched and deleted.</param>
        /// <remarks>
        /// This method uses parallel processing with a maximum of 4 concurrent tasks to enhance performance.
        /// After deletion, a log entry is created indicating the number of files removed.
        /// </remarks>
        public static void RemoveAll(string directoryPath)
        {
            var xmbFiles = Directory.EnumerateFiles(directoryPath, "*.xmb", SearchOption.AllDirectories).ToArray();
            int fileCount = xmbFiles.Length;

            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 4 };

            Parallel.ForEach(xmbFiles, parallelOptions, File.Delete);

            Logger.Log(Logger.LogType.Info, LoggerSource, $"XMB file cleanup completed. {fileCount} files removed.");
        }
    }
}