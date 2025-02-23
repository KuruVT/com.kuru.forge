using System.Collections.Generic;

using Forge.Project;

using UnityEditor;

using UnityEngine;

public class ProjectWindow : EditorWindow
{
    private List<ProjectData> projects;
    private Vector2 scrollPosition;

    [MenuItem("Forge/Project")]
    public static void ShowWindow()
    {
        ProjectWindow wnd = GetWindow<ProjectWindow>();
        wnd.titleContent = new GUIContent("Projects");
        wnd.minSize = new Vector2(400, 300);
    }

    private void OnEnable()
    {
        projects = ProjectManager.LoadAllProjects();
    }

    private void OnFocus()
    {
        projects = ProjectManager.LoadAllProjects();
        Repaint();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Create New Project", GUILayout.Height(40)))
        {
            ImportWindow.ShowWindow();
        }

        GUILayout.Space(20);
        GUILayout.Label("Available Projects", EditorStyles.boldLabel);

        DrawProjectsList();
    }

    private void DrawProjectsList()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));

        if (projects != null && projects.Count > 0)
        {
            foreach (var project in projects)
            {
                if (project == null) continue;

                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.BeginHorizontal();

                Texture2D icon = project.GetIcon();
                GUILayout.Label(icon != null ? new GUIContent(icon) : new GUIContent("No Icon"), GUILayout.Width(64), GUILayout.Height(64));


                EditorGUILayout.BeginVertical();
                GUILayout.Label(project.ProjectName, EditorStyles.boldLabel);
                GUILayout.Label($"{project.Version}");
                GUI.enabled = false;
                EditorGUILayout.TextArea(project.Description, EditorStyles.textArea, GUILayout.Height(50));
                GUI.enabled = true;


                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(2);

                if (GUILayout.Button("Load Project", GUILayout.Height(30)))
                {
                    LoadProject(project.ProjectName);
                }

                EditorGUILayout.EndVertical();
                GUILayout.Space(2);
            }
        }
        else
        {
            GUILayout.Label("No projects available.", EditorStyles.miniLabel);
        }

        EditorGUILayout.EndScrollView();
    }

    private void LoadProject(string projectName)
    {
        Debug.Log($"Loading project: {projectName}");
    }
}
