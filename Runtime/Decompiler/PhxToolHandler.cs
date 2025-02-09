using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using UnityEngine;

namespace Forge
{
    public static class PhxToolHandler
    {
        private const string LoggerSource = "PhxToolHandler";

        /// <summary>
        /// Retrieves the file path to the PhxTool executable.
        /// </summary>
        /// <returns>The file path to PhxTool.exe if found, otherwise an empty string.</returns>
        public static string GetToolPath()
        {
            string assetPath = Path.Combine(Application.dataPath, "com.kuru.forge/Tools/PhxTool/PhxTool.exe");
            string packagePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Library/PackageCache/com.kuru.forge/Tools/PhxTool/PhxTool.exe");

            string toolPath = File.Exists(assetPath) ? assetPath : File.Exists(packagePath) ? packagePath : string.Empty;

            if (string.IsNullOrEmpty(toolPath))
                Logger.Log(Logger.LogType.Error, LoggerSource, "Critical Error: PhxTool.exe not found. Aborting decompilation.");

            return toolPath;
        }

        /// <summary>
        /// Executes the PhxTool with specified arguments.
        /// </summary>
        /// <param name="toolPath">The full path to the PhxTool executable.</param>
        /// <param name="arguments">Command-line arguments to be passed to PhxTool.</param>
        /// <returns>A task that represents the asynchronous execution of the process.</returns>
        public static async Task RunPhxTool(string toolPath, string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = toolPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(toolPath)
            };

            using (var process = Process.Start(psi))
            {
                await Task.Run(() => process.WaitForExit());
            }
        }
    }
}