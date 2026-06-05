using UnityEditor;
using UnityEngine;

public class GitSettingsProvider : SettingsProvider
{
    private const string WarningPrefKey = "GitWindow_PlayModeWarning";

    public GitSettingsProvider(string path, SettingsScope scope)
        : base(path, scope) {}

    public override void OnGUI(string searchContext)
    {
        GUILayout.Label("Git Window", EditorStyles.boldLabel);

        bool enabled = EditorPrefs.GetBool(WarningPrefKey, false);

        bool newValue = EditorGUILayout.Toggle(
            "Warn About Uncommitted Changes Before Play Mode",
            enabled
        );

        if (newValue != enabled)
        {
            EditorPrefs.SetBool(WarningPrefKey, newValue);
        }
    }

    [SettingsProvider]
    public static SettingsProvider CreateProvider()
    {
        return new GitSettingsProvider(
            "Project/Git Window",
            SettingsScope.Project
        );
    }
}