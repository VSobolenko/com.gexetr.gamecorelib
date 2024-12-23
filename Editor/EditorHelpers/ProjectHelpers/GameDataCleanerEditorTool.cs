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
/// <summary>
///to quickly create a data clearing window, just inherit from this class and add the following code:
/// [MenuItem(DefaultHeader, false)]
/// private static void ShowWindow() => ShowDataCleanerWindow<YOUR_WINDOW_CLASS>();
///
///with setup:
/// [MenuItem(DefaultHeader, false)]
/// private static void ShowWindow() => ShowDataCleanerWindow<YOUR_WINDOW_CLASS>(startupConfigure: window =>
/// {
///     window.confirm = true;
///     window.showHeader = false;
/// });
/// 
///to add custom Buttons and Labels:
/// public override void AddSetups() { AddButton(new ButtonData() { } ).AddLabel("..."); }
/// 
/// </summary>

public class GameDataCleaner : EditorWindow
{
    protected const string DefaultHeader = GameData.EditorName + EditorToolsSubfolder.Project + "/Turbo data";
    private readonly HashSet<ButtonData> _buttons = new(2);
    private readonly HashSet<string> _labels = new(2);
    public bool confirm = false;
    public bool showHeader = true;

    protected static T ShowDataCleanerWindow<T>(string title = "Data manager", Action<T> startupConfigure = null)
        where T : GameDataCleaner
    {
        var window = GetWindow<T>();
        window.titleContent = new GUIContent(title);
        window.Show();
        startupConfigure?.Invoke(window);

        return GetWindow<T>();
    }

    private void OnEnable()
    {
        SetupPlayerPrefs(this);
        AddSetups();
    }

    protected static void SetupPlayerPrefs<T>(T window) where T : GameDataCleaner
    {
        window.AddButton(new ButtonData
        {
            description = "Clear PlayerPrefs",
            action = PlayerPrefs.DeleteAll,
        }).AddLabel($@"PlayerPrefs: HKCU\Software\{PlayerSettings.companyName}\{Application.productName}");
    }

    protected virtual void AddSetups() { }
    
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (showHeader && GUILayout.Button("Clear Game Data (Optional)"))
            ProcessClearGameData();
        confirm = GUILayout.Toggle(confirm, "Confirm", GUILayout.Width(65));
        GUILayout.EndHorizontal();
        GUILayout.Space(showHeader ? 15 : 0);

        DrawCleaningPoint();
        DrawCustomCleaningPoint();

        DrawCleaningPointsLabels();
        DrawCustomCleaningPointsLabels();
    }

    private void DrawCleaningPoint()
    {
        foreach (var buttonData in _buttons)
        {
            if (GUILayout.Button(buttonData.description))
                ProcessClearButtonData(buttonData);
            GUILayout.Space(5);
        }
    }

    private void DrawCleaningPointsLabels()
    {
        foreach (var label in _labels)
            EditorGUILayout.SelectableLabel(label, GUILayout.Height(13));
            // EditorGUILayout.SelectableLabel(label, EditorStyles.textField); - custom type
    }

    public GameDataCleaner AddButton(ButtonData button)
    {
        _buttons.Add(button);

        return this;
    }

    public GameDataCleaner AddLabel(string label)
    {
        _labels.Add(label);

        return this;
    }

    private GameDataCleaner ProcessClearGameData()
    {
        if (confirm && ConfirmAction("Game Data", "Clear Game data?") == false)
            return this;

        foreach (var button in _buttons)
            ProcessClearButtonData(button, considerConfirm: false);
        OnClearGameData();
        Log.Info("Game data cleared!");
        return this;
    }

    private void ProcessClearButtonData(ButtonData button, bool considerConfirm = true)
    {
        if (considerConfirm && confirm && ConfirmAction("Confirm action", button.description + "?") == false)
            return;
        button.action?.Invoke();
        OnClearData(button);
    }

    protected virtual void OnClearGameData() { }
    
    protected virtual void OnClearData(ButtonData data) { }

    protected virtual void DrawCustomCleaningPoint() { }

    protected virtual void DrawCustomCleaningPointsLabels() { }

    protected static bool ConfirmAction(string title, string message, string okLabel = "OK",
                                        string cancelLabel = "Cancel")
    {
        return EditorUtility.DisplayDialog(title, message, okLabel, cancelLabel);
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