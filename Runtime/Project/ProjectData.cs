using System;

using UnityEngine;

namespace Forge
{
    public class ProjectData
    {
        /// <summary>
        /// Gets or sets the name of the project.
        /// The name must not exceed 64 characters.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the version of the project.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the description of the project.
        /// The description must not exceed 4000 characters.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the project icon encoded in Base64 format.
        /// The icon must have a 1:1 aspect ratio and a maximum size of 512x512 pixels.
        /// </summary>
        public string IconBase64 { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectData"/> class.
        /// </summary>
        public ProjectData() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectData"/> class with specified project details.
        /// </summary>
        /// <param name="projectName">The name of the project. Must not exceed 64 characters.</param>
        /// <param name="version">The version of the project.</param>
        /// <param name="description">The description of the project. Must not exceed 4000 characters.</param>
        /// <param name="icon">The icon of the project as a <see cref="Texture2D"/> object. Must have a 1:1 aspect ratio and a maximum size of 512x512 pixels.</param>
        public ProjectData(string projectName, string version, string description, Texture2D icon)
        {
            ProjectName = projectName;
            Version = version;
            Description = description;
            IconBase64 = icon != null ? Convert.ToBase64String(icon.EncodeToPNG()) : string.Empty;
        }

        /// <summary>
        /// Retrieves the project icon as a <see cref="Texture2D"/> object from the Base64 encoded string.
        /// </summary>
        /// <returns>The project icon as a <see cref="Texture2D"/>, or null if no icon is set.</returns>
        public Texture2D GetIcon()
        {
            if (!string.IsNullOrEmpty(IconBase64))
            {
                byte[] imageBytes = Convert.FromBase64String(IconBase64);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(imageBytes);
                return texture;
            }
            return null;
        }
    }
}
