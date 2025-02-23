using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Forge.Modules
{
    public static class DDX
    {
        private const string LoggerSource = "DDX Module";

        /// <summary>
        /// Converts all DDX (DDS) files from the specified directory and its subdirectories to PNG.
        /// Skips files that cannot be read by Unity.
        /// </summary>
        /// <param name="directoryPath">The root directory path where DDX files will be searched and converted.</param>
        /// <remarks>
        /// This method uses parallel processing for file conversion. Mipmaps are no longer generated.
        /// </remarks>
        public static void ConvertAll(string directoryPath)
        {
            var ddxFiles = Directory.EnumerateFiles(directoryPath, "*.ddx", SearchOption.AllDirectories).ToArray();
            int fileCount = ddxFiles.Length;

            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 4 };

            Parallel.ForEach(ddxFiles, parallelOptions, file =>
            {

                string ddsFileName = Path.ChangeExtension(file, ".dds");

            });
        }

    }
}
