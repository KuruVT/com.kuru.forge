﻿using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

namespace Forge
{
    public static class Decompiler
    {
        private static readonly string ToolPath = Path.Combine(Application.dataPath, "com.kuru.forge/Tools/PhxTool/PhxTool.exe");

        public static async Task FromEra(string[] files, string outputPath)
        {
            if (!File.Exists(ToolPath))
                return;

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileName(files[i]);
                float progress = (float)i / files.Length;

                EditorUtility.DisplayProgressBar("Decompiling ERA Files", $"Decrypting {fileName}...", progress);
                await ProcessEraFile(files[i], outputPath);
            }

            EditorUtility.ClearProgressBar();
        }

        private static async Task ProcessEraFile(string file, string outputPath)
        {
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
            string fileDir = Path.GetDirectoryName(file);
            string tempDir = Path.Combine(outputPath, "temp");

            if (!Directory.Exists(tempDir))
                Directory.CreateDirectory(tempDir);

            EditorUtility.DisplayProgressBar("Decompiling ERA Files", $"Decrypting {fileNameWithoutExt}...", 0.5f);
            string decryptArgs = $"--env=phx --tool=era --mode=decrypt --path=\"{fileDir}\" --name={fileNameWithoutExt} --out=\"{tempDir}\"";
            await RunPhxTool(decryptArgs);

            string decryptedFile = Path.Combine(tempDir, fileNameWithoutExt + ".era.bin");
            if (!File.Exists(decryptedFile))
                return;

            EditorUtility.DisplayProgressBar("Decompiling ERA Files", $"Expanding {fileNameWithoutExt}...", 0.9f);
            string expandArgs = $"--env=phx --tool=era --mode=expand --path=\"{tempDir}/{fileNameWithoutExt}\" --name={fileNameWithoutExt} --out=\"{outputPath}\"";
            await RunPhxTool(expandArgs);

            Directory.Delete(tempDir, true);
        }

        private static async Task RunPhxTool(string arguments)
        {
            await Task.Run(() =>
            {
                string toolDir = Path.GetDirectoryName(ToolPath);

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = ToolPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = toolDir
                };

                using (Process process = Process.Start(psi))
                {
                    process.WaitForExit();
                }
            });
        }
    }
}
