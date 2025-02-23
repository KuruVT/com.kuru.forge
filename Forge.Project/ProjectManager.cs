using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

using UnityEngine;

namespace Forge.Project
{
    public static class ProjectManager
    {
        /// <summary>
        /// The root directory where all projects are stored.
        /// </summary>
        private static readonly string ProjectsRoot = Path.Combine(Directory.GetParent(Application.dataPath)?.FullName ?? string.Empty, "Forge", "Projects");

        /// <summary>
        /// Types of imports that can be performed when creating or updating a project.
        /// </summary>
        public enum ImportType
        {
            Era,
            Folder
        }

        /// <summary>
        /// Loads all existing projects from the projects directory.
        /// </summary>
        /// <returns>A list of <see cref="ProjectData"/> representing all loaded projects.</returns>
        public static List<ProjectData> LoadAllProjects()
        {
            var projects = new List<ProjectData>();

            if (!Directory.Exists(ProjectsRoot))
                Directory.CreateDirectory(ProjectsRoot);

            foreach (var directory in Directory.GetDirectories(ProjectsRoot))
            {
                string metadataPath = Path.Combine(directory, "mod.fmd");
                if (File.Exists(metadataPath))
                {
                    var project = Deserialize<ProjectData>(metadataPath);
                    if (project != null)
                        projects.Add(project);
                }
            }

            return projects;
        }

        /// <summary>
        /// Saves project metadata to the corresponding project directory in XML format.
        /// </summary>
        /// <param name="project">The <see cref="ProjectData"/> object to be saved.</param>
        public static void SaveProject(ProjectData project)
        {
            string projectPath = Path.Combine(ProjectsRoot, project.ProjectName);
            if (!Directory.Exists(projectPath))
                Directory.CreateDirectory(projectPath);

            string metadataPath = Path.Combine(projectPath, "mod.fmd");
            Serialize(project, metadataPath);
        }

        /// <summary>
        /// Creates a new project and imports files based on the selected type.
        /// </summary>
        /// <param name="projectPath">The path where the project will be created.</param>
        /// <param name="type">The type of import to be performed (Era or Folder).</param>
        /// <param name="files">An array of file paths to be imported.</param>
        public static async Task Create(string projectPath, ImportType type, string[] files)
        {
            if (!Directory.Exists(projectPath))
                Directory.CreateDirectory(projectPath);

            await Import(type, files, projectPath);
        }

        /// <summary>
        /// Imports files into the project directory based on the import type.
        /// </summary>
        /// <param name="type">The type of import to be performed.</param>
        /// <param name="files">An array of file paths to be imported.</param>
        /// <param name="projectPath">The path of the project directory where files will be imported.</param>
        public static async Task Import(ImportType type, string[] files, string projectPath)
        {
            switch (type)
            {
                case ImportType.Era:
                    await Forge.Decompiler.Decompiler.FromEra(files, projectPath);
                    break;

                case ImportType.Folder:
                    ImportFolder(files, projectPath);
                    break;
            }

            await Task.Yield();
        }

        /// <summary>
        /// Copies folders and their contents into the project directory.
        /// </summary>
        /// <param name="folders">An array of folder paths to be copied.</param>
        /// <param name="projectPath">The destination project directory.</param>
        private static void ImportFolder(string[] folders, string projectPath)
        {
            foreach (var folder in folders)
            {
                var folderName = Path.GetFileName(folder);
                var destFolder = Path.Combine(projectPath, folderName);

                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);

                foreach (var file in Directory.GetFiles(folder, "*", SearchOption.AllDirectories))
                {
                    var relativePath = Path.GetRelativePath(folder, file);
                    var destFile = Path.Combine(destFolder, relativePath);

                    Directory.CreateDirectory(Path.GetDirectoryName(destFile));
                    File.Copy(file, destFile, true);
                }
            }
        }

        /// <summary>
        /// Exports the project to a specified path. The export logic should be implemented based on the desired format.
        /// </summary>
        /// <param name="projectPath">The path of the project to be exported.</param>
        /// <param name="exportPath">The destination path where the project will be exported.</param>
        public static async Task Export(string projectPath, string exportPath)
        {
            // Implement export logic here (e.g., zip files, package formats)
            await Task.Yield();
        }

        /// <summary>
        /// Serializes an object to an XML file.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="filePath">The file path where the XML will be saved.</param>
        private static void Serialize<T>(T obj, string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, obj);
            }
        }

        /// <summary>
        /// Deserializes an XML file to an object.
        /// </summary>
        /// <typeparam name="T">The type of the object to deserialize.</typeparam>
        /// <param name="filePath">The file path of the XML to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        private static T Deserialize<T>(string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var reader = new StreamReader(filePath))
            {
                return (T)serializer.Deserialize(reader);
            }
        }
    }
}
