using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.DynamicData;
using Game.Extensions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameEditor.SceneTools
{
internal class SceneSwitcher : EditorWindow
{
    private const string HeaderName = "Scene Loader";

    [MenuItem(GameData.EditorName + EditorSubfolder.Scene + "/" + HeaderName)]
    private static void OpenWindow() => ShowWindow<SceneSwitcher>();

    protected static T ShowWindow<T>(string title = HeaderName, Action<T> startupConfigure = null)
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
        GUILayout.BeginVertical();
        DrawSceneLoaderButtons();
        DrawStartedSceneSelection();
        GUILayout.EndVertical();
        CollectDragAndDropScenes();
    }

    private static void DrawSceneLoaderButtons()
    {
        if (EditorBuildSettings.scenes.Length == 0)
            GUILayout.Label("No Scenes in Build Settings");
        foreach (var scene in EditorBuildSettings.scenes)
        {
            GUILayout.BeginHorizontal();
            var isSceneAssetExists = IsSceneExists(scene).With(x => GUI.enabled = false, x => x == false);
            GUI.color = scene.enabled && isSceneAssetExists ? Color.white : new Color(0.75f, 0.75f, 0.75f);

            if (GUILayout.Button($"\u2196", GUILayout.ExpandWidth(false))) // Select ↖
                SelectSceneFromBuildSettings(scene);

            if (GUILayout.Button($"Load {GetSceneNameWithoutExtension(scene)}"))
            {
                OpenScene(scene);
                GUIUtility.ExitGUI();
            }

            ToggleSceneEnabled(scene, GUILayout.Toggle(scene.enabled && isSceneAssetExists, "", GUILayout.ExpandWidth(false))); // Enable ✓
            
            GUI.enabled = true;
            
            if (GUILayout.Button($"\u2193", GUILayout.ExpandWidth(false)))  // Up ↑
                MoveSceneToShiftIndex(scene, 1);
            if (GUILayout.Button($"\u2191", GUILayout.ExpandWidth(false))) // Down ↓
                MoveSceneToShiftIndex(scene, -1);
            if (GUILayout.Button($"\u00d7", GUILayout.ExpandWidth(false))) // Remove ×
                RemoveSceneFromBuildSettings(scene);
            
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
        }
    }

    private static void DrawStartedSceneSelection()
    {
        const string defaultSceneLoadMode = "Default Load Mode";
        var buttonStyle = new GUIStyle(GUI.skin.button)
        {
            richText = true,
        };
        var text = $"<color=#00FF00>Loading Start Scene</color>: {GetLoadingStartSceneName(defaultSceneLoadMode)}";
        if (GUILayout.Button(new GUIContent(text), buttonStyle))
        {
            var options = GetSceneListForCustomMenu(defaultSceneLoadMode);
            var selectedIndex = GetLoadModeSelectedIndex(options);
            EditorUtility.DisplayCustomMenu(new Rect(Event.current.mousePosition, Vector2.zero), options,
                                            selectedIndex,
                                            UpdateLoadedStartedScene, null);
        }
    }

    private static int GetLoadModeSelectedIndex(GUIContent[] options)
    {
        if (EditorSceneManager.playModeStartScene == null)
            return 0;

        for (var i = 1; i < EditorBuildSettings.scenes.Length + 1; i++)
        {
            var sceneName = EditorSceneManager.playModeStartScene.name;

            if (sceneName == options[i].text)
                return i;
        }

        return -1;
    }

    private static void UpdateLoadedStartedScene(object userData, string[] options, int selected)
    {
        if (selected == 0)
        {
            EditorSceneManager.playModeStartScene = null;

            return;
        }

        for (var i = 1; i < EditorBuildSettings.scenes.Length + 1; i++)
        {
            var scene = EditorBuildSettings.scenes[i - 1];
            var sceneName = GetSceneNameWithoutExtension(scene);
            if (sceneName == options[selected])
            {
                EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path);

                return;
            }
        }
    }

    private static string GetLoadingStartSceneName(string defaultMode) =>
        EditorSceneManager.playModeStartScene == null ? defaultMode : EditorSceneManager.playModeStartScene.name;

    private static GUIContent[] GetSceneListForCustomMenu(string defaultMode)
    {
        var possibleScenesToLoad = new GUIContent[EditorBuildSettings.scenes.Length + 1];
        possibleScenesToLoad[0] = new GUIContent(defaultMode);
        for (var i = 1; i < possibleScenesToLoad.Length; i++)
        {
            var scene = EditorBuildSettings.scenes[i - 1];
            var sceneName = GetSceneNameWithoutExtension(scene);
            possibleScenesToLoad[i] = new GUIContent(sceneName);
        }

        return possibleScenesToLoad;
    }

    private static void OpenScene(EditorBuildSettingsScene scene)
    {
        if (EditorApplication.isPlaying)
        {
            Log.Warning($"\"Can't load \"{GetSceneNameWithoutExtension(scene)}\" in play mode!");

            return;
        }

        if (IsSceneExists(scene) == false)
        {
            Log.Warning($"\"{GetSceneNameWithoutExtension(scene)}\" not found!");

            return;
        }

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(scene.path);
    }

    private static void ToggleSceneEnabled(EditorBuildSettingsScene scene, bool sceneEnableToggle)
    {
        if (sceneEnableToggle == scene.enabled)
            return;

        var index = FindIndex(scene);
        var sceneArray = EditorBuildSettings.scenes;
        sceneArray[index].enabled = sceneEnableToggle;
        EditorBuildSettings.scenes = sceneArray;
    }

    private static void MoveSceneToShiftIndex(EditorBuildSettingsScene scene, int n)
    {
        var index = FindIndex(scene);

        if (index < 0 || index >= EditorBuildSettings.scenes.Length)
            return;

        var newIndex = index + n;

        if (newIndex < 0 || newIndex >= EditorBuildSettings.scenes.Length)
            return;

        var sceneArray = EditorBuildSettings.scenes;
        if (newIndex > index)
            for (var i = index; i < newIndex; i++)
                sceneArray[i] = sceneArray[i + 1];
        else
            for (var i = index; i > newIndex; i--)
                sceneArray[i] = sceneArray[i - 1];

        sceneArray[newIndex] = scene;
        EditorBuildSettings.scenes = sceneArray;
    }

    private static void RemoveSceneFromBuildSettings(EditorBuildSettingsScene scene) => EditorBuildSettings.scenes =
        EditorBuildSettings.scenes.ToList().Where(x => x.guid != scene.guid).ToArray();

    private static void SelectSceneFromBuildSettings(EditorBuildSettingsScene scene) => Selection.activeObject =
        AssetDatabase.LoadAssetAtPath<Object>(scene.path)
                     .With(_ => Log.Warning($"\"{GetSceneNameWithoutExtension(scene)}\" not found!"),
                           when: x => x == null);

    private void CollectDragAndDropScenes()
    {
        var fullArea = new Rect(0, 0, position.width, position.height);
        var evt = Event.current;

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (fullArea.Contains(evt.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (var draggedObject in DragAndDrop.objectReferences)
                        {
                            var path = AssetDatabase.GetAssetPath(draggedObject);

                            if (path.EndsWith(".unity"))
                                AddSceneToBuildSettings(path);
                            else
                                Log.Warning("The object is not a scene: " + path);
                        }

                        evt.Use();
                    }
                }

                break;
        }
    }

    private static void AddSceneToBuildSettings(string scenePath)
    {
        var scenesInBuildSettings = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        var sceneAlreadyAdded = scenesInBuildSettings.Exists(scene => scene.path == scenePath);

        if (!sceneAlreadyAdded)
        {
            scenesInBuildSettings.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenesInBuildSettings.ToArray();
        }
        else
            Log.Warning($"\"{GetSceneNameWithoutExtension(scenePath)}\" is already in Build Settings: " + scenePath);
    }

    private static int FindIndex(EditorBuildSettingsScene scene)
    {
        for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            if (EditorBuildSettings.scenes[i].guid == scene.guid)
                return i;

        return -1;
    }

    private static string GetSceneNameWithoutExtension(EditorBuildSettingsScene scene) =>
        System.IO.Path.GetFileNameWithoutExtension(scene.path);

    private static string GetSceneNameWithoutExtension(string scenePath) =>
        System.IO.Path.GetFileNameWithoutExtension(scenePath);

    private static bool IsSceneExists(EditorBuildSettingsScene scene) =>
        AssetDatabase.LoadAssetAtPath<Object>(scene.path) != null;
}
}