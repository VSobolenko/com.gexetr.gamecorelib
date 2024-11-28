using System;
using Game.DynamicData;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameEditor.Tools
{
public class SceneSwitcher : EditorWindow
{
    private EditorBuildSettingsScene[] _scenes;

    [MenuItem(GameData.EditorName + "/Scene Loader")]
    private static void OpenWindow() => ShowWindow<SceneSwitcher>();

    protected static T ShowWindow<T>(string title = "Scene Loader", Action<T> startupConfigure = null)
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
        UpdateSceneCollection();
        GUILayout.BeginVertical();
        foreach (var scene in _scenes)
        {
            var sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
            if (GUILayout.Button($"Load {sceneName}"))
                OpenScene(scene);
        }

        GUILayout.EndVertical();
    }

    private void UpdateSceneCollection() => _scenes = EditorBuildSettings.scenes;

    private static void OpenScene(EditorBuildSettingsScene scene)
    {
        if (EditorApplication.isPlaying)
            return;
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(scene.path);
    }
}
}