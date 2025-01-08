using System;
using System.Collections.Generic;
using System.IO;
using Game.DynamicData;
using Game;
using UnityEditor;
using UnityEngine;

// ReSharper disable StringLiteralTypo

namespace GameEditor.ProjectTools
{
/// <example>
/// <para>to quickly create a data clearing window, just inherit from this class and add the following code.</para>
/// <code>
/// [MenuItem(DefaultHeader, false)]
/// private static void ShowWindow() => ShowWindow<ActionsWindowEditorTool>();
///
/// <para>with setup.</para>
/// [MenuItem(DefaultHeader, false)]
/// private static void ShowWindow() => ShowWindow<ActionsWindowEditorTool>(startupConfigure: window =>
/// {
///     window.confirm = true;
///     window.showHeader = false;
/// });
/// 
/// <para>to add custom Buttons and Labels.</para>
/// public override void AddSetups() { AddButton(new ButtonData() { } ).AddLabel("..."); }
/// </code>
/// </example>

public class ActionsWindowEditorTool : EditorWindow
{
    protected const string DefaultHeader = GameData.EditorName + EditorToolsSubfolder.Project + "/Turbo Actions";
    private readonly HashSet<ButtonData> _buttons = new(2);
    private readonly HashSet<string> _labels = new(2);
    public bool confirm = false;
    public bool showHeader = true;

    protected static T ShowWindow<T>(string title = "Turbo Actions", Action<T> startupConfigure = null)
        where T : ActionsWindowEditorTool
    {
        var window = GetWindow<T>();
        window.titleContent = new GUIContent(title);
        window.Show();
        startupConfigure?.Invoke(window);

        return GetWindow<T>();
    }

    private void OnEnable() => AddSetups();

    protected virtual void AddSetups() { }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (showHeader && GUILayout.Button("Execute All Actions (Optional)"))
            ProcessEditorActions();
        confirm = GUILayout.Toggle(confirm, "Confirm", GUILayout.Width(65));
        GUILayout.EndHorizontal();
        GUILayout.Space(showHeader ? 15 : 0);

        DrawEditorActionButtons();
        DrawCustomEditorActions();

        DrawLabels();
        DrawCustomCleaningPointsLabels();
    }

    private void DrawEditorActionButtons()
    {
        foreach (var buttonData in _buttons)
        {
            if (GUILayout.Button(buttonData.description))
                ProcessEditorAction(buttonData);
            GUILayout.Space(5);
        }
    }

    private void DrawLabels()
    {
        foreach (var label in _labels)
            EditorGUILayout.SelectableLabel(label, GUILayout.Height(13));
            // EditorGUILayout.SelectableLabel(label, EditorStyles.textField); - custom type
    }

    public ActionsWindowEditorTool AddButton(ButtonData button)
    {
        _buttons.Add(button);

        return this;
    }

    public ActionsWindowEditorTool AddLabel(string label)
    {
        _labels.Add(label);

        return this;
    }

    protected ActionsWindowEditorTool ProcessEditorActions()
    {
        if (Application.isPlaying)
        {
            Log.Warning("Unable to execute action in Play Mode");
            return this;
        }
        
        if (confirm && ConfirmAction("Turbo Actions", "Execute?") == false)
            return this;

        foreach (var button in _buttons)
            ProcessEditorAction(button, independentAction: false);
        OnEditorActions();
        Log.Info("Execute Actions Success!");
        return this;
    }

    private void ProcessEditorAction(ButtonData button, bool independentAction = true)
    {
        if (Application.isPlaying)
        {
            Log.Warning("Unable to execute action in Play Mode");
            return;
        }
        
        if (independentAction && confirm && ConfirmAction("Confirm action", button.description + "?") == false)
            return;
        button.action?.Invoke();
        if (independentAction)
            Log.Info($"\"{button.description}\" - Execute Success!");
        OnEditorAction(button);
    }

    protected virtual void OnEditorActions() { }

    protected virtual void OnEditorAction(ButtonData data) { }

    protected virtual void DrawCustomEditorActions() { }

    protected virtual void DrawCustomCleaningPointsLabels() { }

    protected static bool ConfirmAction(string title, string message, string okLabel = "OK",
                                        string cancelLabel = "Cancel")
    {
        return EditorUtility.DisplayDialog(title, message, okLabel, cancelLabel);
    }

    public ActionsWindowEditorTool AddClearPlayerPrefsSetup()
    {
        AddButton(new ButtonData
        {
            description = "Clear PlayerPrefs",
            action = PlayerPrefs.DeleteAll,
        }).AddLabel($@"PlayerPrefs: HKCU\Software\{PlayerSettings.companyName}\{Application.productName}");
        return this;
    }

    public ActionsWindowEditorTool AddPlaySetup()
    {
        AddButton(new ButtonData
        {
            description = "Play",
            action = () => EditorApplication.isPlaying = true,
        });
        return this;
    }
    
    public bool DeleteFolder(string path, bool log = true)
    {
        if (Directory.Exists(path) == false) 
            return false;
        Directory.Delete(path, true);
        if (log)
            Log.Info($"Directory Success Delete: {path}");
        return true;
    }

    public bool DeleteFile(string filePath, bool log = true)
    {
        if (File.Exists(filePath) == false) 
            return false;
        File.Delete(filePath);
        if (log)
            Log.Info($"File Success Delete: {filePath}");
        return true;
    }
    
    public struct ButtonData
    {
        public string description;
        public Action action;
    }
}
}