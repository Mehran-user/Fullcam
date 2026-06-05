using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Diagnostics;
using System.IO;
using Debug = UnityEngine.Debug;

public class GitWindow : EditorWindow
{
    private string commitMessage = "Updated project";
    private Vector2 scroll;
    private string output = "";
    private string gitStatus = "";
    private string currentBranch = "Unknown";
    private string commitHistory = "";

    private bool hasUncommittedChanges;

    private const string WarningPrefKey = "GitWindow_PlayModeWarning";

    [MenuItem("Tools/Git Window")]
    public static void ShowWindow()
    {
        GetWindow<GitWindow>("Git Window");
    }

    private void OnEnable()
    {
        RefreshAll();
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
    }

    private void OnGUI()
    {
        GUILayout.Space(8);

        DrawHeader();

        GUILayout.Space(10);

        DrawMainControls();

        GUILayout.Space(10);

        DrawStatusSection();

        GUILayout.Space(10);

        DrawHistorySection();

        GUILayout.Space(10);

        DrawConsole();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("Repository", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("Current Branch", currentBranch);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Refresh"))
        {
            RefreshAll();
        }

        if (GUILayout.Button("Open Repo Folder"))
        {
            EditorUtility.RevealInFinder(ProjectRoot());
        }

        if (GUILayout.Button("Open GitHub"))
        {
            OpenRemoteRepo();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawMainControls()
    {
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("Source Control", EditorStyles.boldLabel);

        commitMessage = EditorGUILayout.TextField("Commit Message", commitMessage);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Fetch", GUILayout.Height(28)))
            RunGitCommand("fetch");

        if (GUILayout.Button("Pull", GUILayout.Height(28)))
            RunGitCommand("pull");

        if (GUILayout.Button("Commit", GUILayout.Height(28)))
            CommitChanges();

        GUI.enabled = HasCommitsToPush();

        if (GUILayout.Button("Push", GUILayout.Height(28)))
            RunGitCommand("push");

        GUI.enabled = true;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(
            "Commit runs on the main thread intentionally so Unity can safely display progress dialogs.",
            MessageType.Info
        );

        EditorGUILayout.EndVertical();
    }

    private void DrawStatusSection()
    {
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("Git Status", EditorStyles.boldLabel);

        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(140));
        EditorGUILayout.TextArea(gitStatus);
        EditorGUILayout.EndScrollView();

        EditorGUILayout.EndVertical();
    }

    private void DrawHistorySection()
    {
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("Recent Commits", EditorStyles.boldLabel);

        EditorGUILayout.TextArea(commitHistory, GUILayout.Height(100));

        EditorGUILayout.EndVertical();
    }

    private void DrawConsole()
    {
        EditorGUILayout.BeginVertical("box");

        GUILayout.Label("Console", EditorStyles.boldLabel);

        EditorGUILayout.TextArea(output, GUILayout.Height(120));

        EditorGUILayout.EndVertical();
    }

    private void CommitChanges()
    {
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayProgressBar("Git Commit", "Running git commands...", 0.5f);

        RunGitCommand("add .");
        RunGitCommand($"commit -m \"{commitMessage}\"");

        EditorUtility.ClearProgressBar();
    }

    private void RefreshAll()
    {
        currentBranch = RunGitCommandWithResult("branch --show-current");
        gitStatus = RunGitCommandWithResult("status --short");
        commitHistory = RunGitCommandWithResult("log --oneline -10");

        hasUncommittedChanges = !string.IsNullOrWhiteSpace(gitStatus);

        Repaint();
    }

    private bool HasCommitsToPush()
    {
        string result = RunGitCommandWithResult("status -sb");
        return result.Contains("ahead");
    }

    private void OpenRemoteRepo()
    {
        string url = RunGitCommandWithResult("remote get-url origin").Trim();

        if (url.StartsWith("git@github.com:"))
        {
            url = url.Replace("git@github.com:", "https://github.com/");
            url = url.Replace(".git", "");
        }

        Application.OpenURL(url);
    }

    private void OnPlayModeChanged(PlayModeStateChange state)
    {
        bool enabled = EditorPrefs.GetBool(WarningPrefKey, false);

        if (!enabled)
            return;

        if (state == PlayModeStateChange.ExitingEditMode)
        {
            RefreshAll();

            if (hasUncommittedChanges)
            {
                bool continuePlay = EditorUtility.DisplayDialog(
                    "Uncommitted Changes",
                    "You have uncommitted Git changes before entering Play Mode.",
                    "Enter Play Mode",
                    "Cancel"
                );

                if (!continuePlay)
                {
                    EditorApplication.isPlaying = false;
                }
            }
        }
    }

    private void RunGitCommand(string command)
    {
        string result = RunGitCommandWithResult(command);

        output += $"> git {command}\n";
        output += result + "\n";

        RefreshAll();

        Debug.Log(result);
    }

    private string RunGitCommandWithResult(string command)
    {
        try
        {
            Process process = new Process();

            process.StartInfo.FileName = "git";
            process.StartInfo.Arguments = command;
            process.StartInfo.WorkingDirectory = ProjectRoot();

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            string result = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(errors))
                result += "\nERROR:\n" + errors;

            return result;
        }
        catch (System.Exception e)
        {
            return e.Message;
        }
    }

    private string ProjectRoot()
    {
        return Path.GetFullPath(Application.dataPath + "/..");
    }
}