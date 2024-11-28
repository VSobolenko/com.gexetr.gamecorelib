using System;
using System.Collections.Generic;
using Game.DynamicData;
using Game;
using UnityEditor;
using UnityEngine;

// ReSharper disable StringLiteralTypo

namespace GameEditor.Tools
{
/*
to quickly create a data clearing window, just inherit from this class and add the following code:
    [MenuItem(DefaultHeader, false)]
    private static void ShowWindow() => ShowDataCleanerWindow<YOUR_WINDOW_CLASS>();

with setup:
    [MenuItem(DefaultHeader, false)]
    private static void ShowWindow() => ShowDataCleanerWindow<DataCleaner>(startupConfigure: window =>
    {
        window.confirm = false;
        window.showHeader = false;
    });
*/

public class GameDataCleaner : EditorWindow
{
    protected const string DefaultHeader = GameData.EditorName + "/Turbo data";
    private readonly HashSet<ButtonData> _buttons = new(2);
    private readonly HashSet<string> _labels = new(2);
    protected bool confirm = true;
    protected bool showHeader = true;

    protected static T ShowDataCleanerWindow<T>(string title = "Data manager", Action<T> startupConfigure = null)
        where T : GameDataCleaner
    {
        var window = GetWindow<T>();
        window.titleContent = new GUIContent(title);
        window.Show();
        SetupPlayerPrefs(window);
        startupConfigure?.Invoke(window);

        return GetWindow<T>();
    }

    private static void SetupPlayerPrefs<T>(T window) where T : GameDataCleaner
    {
        window.AddButton(new ButtonData
        {
            description = "Clear PlayerPrefs",
            action = PlayerPrefs.DeleteAll,
        }).AddLabel($"PlayerPrefs: HKCU\\Software\\{PlayerSettings.companyName}\\{Application.productName}");
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (showHeader && GUILayout.Button("Clear Game Data (Optional)"))
            ProcessClearGameData();
        confirm = GUILayout.Toggle(confirm, "Confirm", GUILayout.Width(65));
        GUILayout.EndHorizontal();
        GUILayout.Space(showHeader ? 20 : 10);

        foreach (var buttonData in _buttons)
        {
            if (GUILayout.Button(buttonData.description))
                ProcessClearButtonData(buttonData);
            GUILayout.Space(5);
        }

        DrawCustomCleaningPoint();

        foreach (var label in _labels)
            GUILayout.Label(label);
        DrawCustomLabels();
    }

    protected GameDataCleaner AddButton(ButtonData button)
    {
        _buttons.Add(button);

        return this;
    }

    protected GameDataCleaner AddLabel(string label)
    {
        _labels.Add(label);

        return this;
    }

    private void ProcessClearGameData()
    {
        if (confirm && ConfirmAction("Game Data", "Clear Game data?") == false)
            return;

        foreach (var button in _buttons)
            ProcessClearButtonData(button, considerConfirm: false);
        OnClearGameData();
        Log.Info("Game data cleared!");
    }

    private void ProcessClearButtonData(ButtonData button, bool considerConfirm = true)
    {
        if (considerConfirm && confirm && ConfirmAction("Confirm action", button.description + "?") == false)
            return;
        button.action?.Invoke();
        OnClearData(button);
    }

    protected virtual void OnClearGameData() { }
    protected virtual void OnClearData(ButtonData button) { }

    protected virtual void DrawCustomCleaningPoint() { }

    protected virtual void DrawCustomLabels() { }

    protected static bool ConfirmAction(string title, string message, string okLabel = "OK",
                                        string cancelLabel = "Cancel")
    {
        return EditorUtility.DisplayDialog(title, message, okLabel, cancelLabel);
    }

    protected struct ButtonData
    {
        public string description;
        public Action action;
    }
}
}