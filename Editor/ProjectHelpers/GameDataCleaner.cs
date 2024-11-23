using System;
using Game.DynamicData;
using Game;
using UnityEditor;
using UnityEngine;

namespace WarehouseKeeper.EditorScripts
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
    protected bool confirm = true;
    protected bool showHeader = false;

    protected static T ShowDataCleanerWindow<T>(string title = "Data manager", Action<T> startupConfigure = null)
        where T : EditorWindow
    {
        var window = GetWindow<T>();
        window.titleContent = new GUIContent(title);
        window.Show();
        startupConfigure?.Invoke(window);

        return GetWindow<T>();
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (showHeader && GUILayout.Button("Clear Game Data (Optional)"))
            PrecessClearGameData();

        confirm = GUILayout.Toggle(confirm, "Confirm", GUILayout.Width(65));
        GUILayout.EndHorizontal();
        GUILayout.Space(showHeader ? 20 : 10);

        if (GUILayout.Button("Clear PlayerPrefs"))
            PrecessClearPlayerPrefs();
        GUILayout.Space(5);

        DrawCustomCleaningPoint();
        DrawCustomDescription();
    }

    private void PrecessClearGameData()
    {
        if (confirm && ConfirmAction("Game Data", "Clear Game data?") == false)
            return;

        ClearPlayerPrefs();
        ClearGameData();
        Log.Info("Game data cleared!");
    }

    private void ClearPlayerPrefs() => PlayerPrefs.DeleteAll();

    protected virtual void ClearGameData() { }

    private void PrecessClearPlayerPrefs()
    {
        if (confirm && ConfirmAction("PlayerPrefs", "Clear PlayerPrefs data?") == false)
            return;

        ClearPlayerPrefs();
        OnClearPlayerPrefs();
        Log.Info("Player prefs cleared!");
    }

    protected virtual void OnClearPlayerPrefs() { }

    protected virtual void DrawCustomDescription() { }

    protected virtual void DrawCustomCleaningPoint() { }

    protected static bool ConfirmAction(string title, string message, string okLabel = "OK",
                                        string cancelLabel = "Cancel")
    {
        return EditorUtility.DisplayDialog(title, message, okLabel, cancelLabel);
    }
}
}