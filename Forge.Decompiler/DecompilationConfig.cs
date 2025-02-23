using System;
using System.IO;
using System.Xml.Serialization;

using UnityEngine;

namespace Forge.Decompiler
{
    [Serializable, XmlRoot("DecompilationConfig")]
    public class DecompilationConfig
    {
        private static readonly string ConfigDirectory = Path.Combine(Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty, "Forge");
        private static readonly string ConfigFilePath = Path.Combine(ConfigDirectory, "DecompilationConfig.xml");
        private const string LoggerSource = "Decompilation Config";

        [XmlElement("CleanUpXmb")]
        public bool CleanUpXmb { get; set; } = true;

        [XmlElement("CleanUpEraDef")]
        public bool CleanUpEraDef { get; set; } = true;

        [XmlElement("ConvertToDds")]
        public bool ConvertToDds { get; set; } = true;

        [XmlElement("EnableParallelProcessing")]
        public bool EnableParallelProcessing { get; set; } = true;

        [XmlElement("ParallelThreads")]
        public int ParallelThreads { get; set; } = 0;

        /// <summary>
        /// Loads the decompilation configuration from a file. If the file does not exist, a default configuration is created and saved.
        /// </summary>
        /// <returns>The loaded or default <see cref="DecompilationConfig"/>.</returns>
        public static DecompilationConfig Load()
        {
            if (!Directory.Exists(ConfigDirectory))
                Directory.CreateDirectory(ConfigDirectory);

            if (File.Exists(ConfigFilePath))
                return Deserialize(File.ReadAllText(ConfigFilePath));

            var defaultConfig = new DecompilationConfig { ParallelThreads = GetDefaultThreadCount() };
            defaultConfig.Save();
            Debug.Log($"[INFO] {LoggerSource}: Default configuration created.");
            return defaultConfig;
        }

        /// <summary>
        /// Saves the current decompilation configuration to a file.
        /// </summary>
        public void Save()
        {
            if (!Directory.Exists(ConfigDirectory))
                Directory.CreateDirectory(ConfigDirectory);

            File.WriteAllText(ConfigFilePath, Serialize(this));
            Debug.Log($"[INFO] {LoggerSource}: Configuration saved.");
        }

        /// <summary>
        /// Calculates the default number of parallel threads based on the processor count.
        /// </summary>
        /// <returns>The default number of threads.</returns>
        private static int GetDefaultThreadCount()
        {
            return Mathf.Max(1, Environment.ProcessorCount / 2);
        }

        /// <summary>
        /// Deserializes the XML string into a <see cref="DecompilationConfig"/> object.
        /// </summary>
        /// <param name="xml">The XML string representing the configuration.</param>
        /// <returns>The deserialized <see cref="DecompilationConfig"/> object.</returns>
        private static DecompilationConfig Deserialize(string xml)
        {
            var serializer = new XmlSerializer(typeof(DecompilationConfig));
            using (var reader = new StringReader(xml))
            {
                return (DecompilationConfig)serializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// Serializes the <see cref="DecompilationConfig"/> object into an XML string.
        /// </summary>
        /// <param name="config">The configuration object to serialize.</param>
        /// <returns>The XML string representing the configuration.</returns>
        private static string Serialize(DecompilationConfig config)
        {
            var serializer = new XmlSerializer(typeof(DecompilationConfig));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, config);
                return writer.ToString();
            }
        }
    }
}
