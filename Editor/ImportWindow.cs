using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Forge;
using SFB;

public class ImportWindow : EditorWindow
{
    private string projectName = "New Project";
    private string projectVersion = "1.0.0";
    private string projectDescription = "Describe your project here.";
    private Texture2D projectIcon = null;

    private List<string> importFiles = new List<string>();
    private bool canApply = false;

    private Vector2 scrollPosition;
    private Vector2 descriptionScrollPosition;

    private const int MaxFileNameLength = 64;
    private const int MaxDescriptionLength = 4000;
    private const int MaxIconSize = 512;

    private bool isProjectNameValid = true;
    private bool isVersionValid = true;
    private bool isDescriptionValid = true;
    private bool isIconValid = true;

    public static void ShowWindow()
    {
        ImportWindow wnd = GetWindow<ImportWindow>();
        wnd.titleContent = new GUIContent("Import Project");
        wnd.minSize = new Vector2(400, 600);
    }

    private void OnGUI()
    {
        GUILayout.Label("Project Details", EditorStyles.boldLabel);
        ValidateInputs();

        DrawProjectNameField();
        DrawVersionField();
        DrawDescriptionField();
        DrawIconSection();

        GUILayout.Space(10);
        DrawImportFilesSection();
        GUILayout.Space(10);

        DrawActionButtons();
    }

    private void DrawProjectNameField()
    {
        projectName = EditorGUILayout.TextField("Project Name", projectName);
        if (!isProjectNameValid)
        {
            EditorGUILayout.HelpBox($"Project name exceeds {MaxFileNameLength} characters!", MessageType.Error);
        }
    }

    private void DrawVersionField()
    {
        projectVersion = EditorGUILayout.TextField("Version", projectVersion);
        if (!isVersionValid)
        {
            EditorGUILayout.HelpBox("Invalid version format. Use Unity's versioning style (e.g., 1.0.0, 2022.1.5f1).", MessageType.Error);
        }
    }

    private void DrawDescriptionField()
    {
        GUILayout.Label("Description", EditorStyles.boldLabel);
        descriptionScrollPosition = EditorGUILayout.BeginScrollView(descriptionScrollPosition, GUILayout.Height(80));
        projectDescription = EditorGUILayout.TextArea(projectDescription, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        if (!isDescriptionValid)
        {
            EditorGUILayout.HelpBox($"Description exceeds {MaxDescriptionLength} characters!", MessageType.Error);
        }
    }

    private void DrawIconSection()
    {
        GUILayout.Space(10);
        if (GUILayout.Button("Select Project Icon", GUILayout.Height(30)))
        {
            SelectProjectIcon();
        }

        if (projectIcon != null)
        {
            GUILayout.Label(projectIcon, GUILayout.Width(64), GUILayout.Height(64));
            if (!isIconValid)
            {
                EditorGUILayout.HelpBox($"Icon must be square (1:1 ratio) and no larger than {MaxIconSize}x{MaxIconSize}.", MessageType.Error);
            }
        }
        else
        {
            GUILayout.Label("No Icon Selected");
        }
    }

    private void DrawImportFilesSection()
    {
        if (GUILayout.Button("Import Files (.era)", GUILayout.Height(30)))
        {
            SelectMultipleFiles();
        }

        GUILayout.Space(5);
        GUILayout.Label("Imported Files", EditorStyles.boldLabel);

        float boxHeight = EditorGUIUtility.singleLineHeight * 6;
        Rect boxRect = GUILayoutUtility.GetRect(0, boxHeight, GUILayout.ExpandWidth(true));
        GUI.Box(boxRect, GUIContent.none);

        scrollPosition = GUI.BeginScrollView(boxRect, scrollPosition, new Rect(0, 0, boxRect.width - 20, importFiles.Count * EditorGUIUtility.singleLineHeight));
        for (int i = 0; i < importFiles.Count; i++)
        {
            Rect labelRect = new Rect(5, i * EditorGUIUtility.singleLineHeight, boxRect.width - 30, EditorGUIUtility.singleLineHeight);
            GUI.Label(labelRect, Path.GetFileName(importFiles[i]));
        }
        GUI.EndScrollView();
    }

    private void DrawActionButtons()
    {
        GUI.enabled = canApply && isProjectNameValid && isVersionValid && isDescriptionValid && isIconValid;

        if (GUILayout.Button("Apply", GUILayout.Height(30)))
        {
            ApplyImport();
        }

        GUILayout.Space(5);

        GUI.enabled = true;
    }

    private void SelectMultipleFiles()
    {
        var extensions = new[] { new ExtensionFilter("ERA Files", "era") };
        string[] files = StandaloneFileBrowser.OpenFilePanel("Select ERA Files", "", extensions, true);

        if (files != null && files.Length > 0)
        {
            importFiles.AddRange(files);
            canApply = importFiles.Count > 0;
        }
    }

    private void SelectProjectIcon()
    {
        string path = EditorUtility.OpenFilePanel("Select Project Icon", "", "png,jpg,jpeg");
        if (!string.IsNullOrEmpty(path))
        {
            byte[] imageData = File.ReadAllBytes(path);
            Texture2D tempIcon = new Texture2D(2, 2);
            tempIcon.LoadImage(imageData);

            projectIcon = tempIcon;  

            isIconValid = (tempIcon.width <= MaxIconSize && tempIcon.height <= MaxIconSize && tempIcon.width == tempIcon.height);
        }
    }

    private void ValidateInputs()
    {
        isProjectNameValid = projectName.Length <= MaxFileNameLength;
        isVersionValid = Regex.IsMatch(projectVersion, @"^\d+\.\d+\.\d+([a-zA-Z0-9]*)?$");
        isDescriptionValid = projectDescription.Length <= MaxDescriptionLength;
    }

    private async void ApplyImport()
    {
        string projectPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Forge", "Projects", projectName);

        await ProjectManager.Create(projectPath, ProjectManager.ImportType.Era, importFiles.ToArray());

        ProjectData newProject = new ProjectData(projectName, projectVersion, projectDescription, projectIcon);
        ProjectManager.SaveProject(newProject);

        Close();
    }
}
