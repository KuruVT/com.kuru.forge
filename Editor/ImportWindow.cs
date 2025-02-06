using System.Collections.Generic;
using System.IO;
using System.Linq;

using Forge;

using SFB;

using UnityEditor;

using UnityEngine;

using static Forge.Project;

public class ImportWindow : EditorWindow
{
    private string projectName = "NewProject";
    private List<string> importFiles = new List<string>();
    private bool canApply = false;

    public static void ShowWindow()
    {
        ImportWindow wnd = GetWindow<ImportWindow>();
        wnd.titleContent = new GUIContent("Import Project");
        wnd.minSize = new Vector2(400, 300);
    }

    private void OnGUI()
    {
        GUILayout.Label("Project Name", EditorStyles.boldLabel);
        projectName = EditorGUILayout.TextField(projectName);

        GUILayout.Space(10);

        if (GUILayout.Button("Import Files (.era)", GUILayout.Height(30)))
        {
            SelectMultipleFiles();
        }

        GUILayout.Space(10);

        GUILayout.Label("Imported Files:", EditorStyles.boldLabel);
        foreach (var file in importFiles)
        {
            GUILayout.Label(Path.GetFileName(file));
        }

        GUILayout.Space(10);

        GUI.enabled = canApply;
        if (GUILayout.Button("Apply", GUILayout.Height(30)))
        {
            ApplyImport();
        }
        GUI.enabled = true;
    }

    private void SelectMultipleFiles()
    {
        var extensions = new[] { new ExtensionFilter("ERA Files", "era") };
        string[] files = StandaloneFileBrowser.OpenFilePanel("Select ERA Files", "", extensions, true);

        if (files != null && files.Length > 0)
        {
            importFiles.AddRange(files.Where(f => f.EndsWith(".era")));
            canApply = importFiles.Count > 0;
        }
    }

    private async void ApplyImport()
    {
        await Project.Create(Path.Combine(Directory.GetParent(Application.dataPath).FullName, projectName), ImportType.Era, importFiles.ToArray());


        Close();
    }
}
