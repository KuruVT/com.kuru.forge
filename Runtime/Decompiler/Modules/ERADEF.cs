using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Forge.Modules
{
    public static class ERADEF
    {
        private const string LoggerSource = "ERADEF Module";

        /// <summary>
        /// Removes all ERADEF files from the specified directory and its subdirectories.
        /// </summary>
        /// <param name="directoryPath">The root directory path where ERADEF files will be searched and deleted.</param>
        /// <remarks>
        /// This method uses parallel processing with a maximum of 4 concurrent tasks to enhance performance.
        /// After deletion, a log entry is created indicating the number of files removed.
        /// </remarks>
        public static void RemoveAll(string directoryPath)
        {
            var eraDefFiles = Directory.EnumerateFiles(directoryPath, "*.eradef", SearchOption.AllDirectories).ToArray();
            int fileCount = eraDefFiles.Length;

            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 4 };

            Parallel.ForEach(eraDefFiles, parallelOptions, File.Delete);

            Logger.Log(Logger.LogType.Info, LoggerSource, $"ERADEF file cleanup completed. {fileCount} files removed.");
        }
    }
}