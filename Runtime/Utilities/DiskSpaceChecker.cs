using System.IO;
using System.Linq;

namespace Forge.Utilities
{
    public static class DiskSpaceChecker
    {
        /// <summary>
        /// Checks if there is sufficient disk space available to process the specified files.
        /// </summary>
        /// <param name="files">An array of file paths to be checked.</param>
        /// <param name="outputPath">The directory where files will be processed or saved.</param>
        /// <returns>True if there is enough disk space, otherwise false.</returns>
        /// <remarks>
        /// The method calculates the total size of the provided files and requires double that amount
        /// of free space to proceed with the operation.
        /// </remarks>
        public static bool HasSufficientDiskSpace(string[] files, string outputPath)
        {
            long totalSize = files.Sum(file => new FileInfo(file).Length);
            long requiredSpace = totalSize * 2;

            DriveInfo drive = new DriveInfo(Path.GetPathRoot(outputPath));
            long availableSpace = drive.AvailableFreeSpace;

            return availableSpace >= requiredSpace;
        }
    }
}
