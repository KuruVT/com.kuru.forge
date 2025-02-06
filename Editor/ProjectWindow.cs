using UnityEditor;

using UnityEngine;

public class ProjectWindow : EditorWindow
{

    [MenuItem("Forge/Project")]
    public static void ShowWindow()
    {
        ProjectWindow wnd = GetWindow<ProjectWindow>();
        wnd.titleContent = new GUIContent("Forge Project");
        wnd.minSize = new Vector2(400, 200);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Create Project", GUILayout.Height(40)))
        {
            ImportWindow.ShowWindow();
        }
    }
}
