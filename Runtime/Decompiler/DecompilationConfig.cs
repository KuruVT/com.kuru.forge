using System.IO;

using UnityEngine;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Forge
{
    [System.Serializable]
    public class DecompilationConfig
    {
        private static readonly string ConfigDirectory = Path.Combine(Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty, "Forge");
        private static readonly string ConfigFilePath = Path.Combine(ConfigDirectory, ".fdc");
        private const string LoggerSource = "Decompilation Config";

        public bool CleanUpXmb { get; set; } = true;
        public bool CleanUpEraDef { get; set; } = true;
        public bool EnableParallelProcessing { get; set; } = true;
        public int ParallelThreads { get; set; } = 0;

        /// <summary>
        /// Loads the decompilation configuration from a file. If the file does not exist, default configuration is created and saved.
        /// </summary>
        /// <returns>The loaded or default <see cref="DecompilationConfig"/>.</returns>
        public static DecompilationConfig LoadConfig()
        {
            if (!Directory.Exists(ConfigDirectory))
                Directory.CreateDirectory(ConfigDirectory);

            if (File.Exists(ConfigFilePath))
                return DeserializeConfig(File.ReadAllText(ConfigFilePath));

            var defaultConfig = new DecompilationConfig { ParallelThreads = GetDefaultThreadCount() };
            defaultConfig.SaveConfig();
            Logger.Log(Logger.LogType.Info, LoggerSource, "Default configuration created.");
            return defaultConfig;
        }

        /// <summary>
        /// Saves the current decompilation configuration to a file.
        /// </summary>
        public void SaveConfig()
        {
            if (!Directory.Exists(ConfigDirectory))
                Directory.CreateDirectory(ConfigDirectory);

            File.WriteAllText(ConfigFilePath, SerializeConfig(this));
            Logger.Log(Logger.LogType.Info, LoggerSource, "Configuration saved.");
        }

        /// <summary>
        /// Calculates the default number of parallel threads based on the processor count.
        /// </summary>
        /// <returns>The default number of threads.</returns>
        private static int GetDefaultThreadCount()
        {
            return Mathf.Max(1, System.Environment.ProcessorCount / 2);
        }

        /// <summary>
        /// Deserializes the YAML string into a <see cref="DecompilationConfig"/> object.
        /// </summary>
        /// <param name="yaml">The YAML string representing the configuration.</param>
        /// <returns>The deserialized <see cref="DecompilationConfig"/> object.</returns>
        private static DecompilationConfig DeserializeConfig(string yaml)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return deserializer.Deserialize<DecompilationConfig>(yaml);
        }

        /// <summary>
        /// Serializes the <see cref="DecompilationConfig"/> object into a YAML string.
        /// </summary>
        /// <param name="config">The configuration object to serialize.</param>
        /// <returns>The YAML string representing the configuration.</returns>
        private static string SerializeConfig(DecompilationConfig config)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();
            return serializer.Serialize(config);
        }
    }
}