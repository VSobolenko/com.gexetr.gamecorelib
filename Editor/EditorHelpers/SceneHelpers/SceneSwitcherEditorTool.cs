using System;
using Game.DynamicData;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GameEditor.SceneTools
{
    public class SceneSwitcher : EditorWindow
    {
        private const string HeaderName = "Scene Loader";

        private EditorBuildSettingsScene[] _scenes;

        [MenuItem(GameData.EditorName + EditorToolsSubfolder.Scene + "/" + HeaderName)]
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
            UpdateSceneCollection();
            GUILayout.BeginVertical();
            DrawSceneLoaderButtons();
            DrawStartedSceneSelection();
            GUILayout.EndVertical();
        }

        private void DrawSceneLoaderButtons()
        {
            if (_scenes.Length == 0)
                GUILayout.Label("No Scenes in Build Settings");
            foreach (var scene in _scenes)
            {
                var sceneName = GetSceneNameWithoutExtension(scene);
                GUI.color = !scene.enabled ? new Color(0.75f, 0.75f, 0.75f) : Color.white;
                if (GUILayout.Button($"Load {sceneName}"))
                {
                    OpenScene(scene);
                    GUIUtility.ExitGUI();
                }

                GUI.color = Color.white;
            }
        }

        private static string GetSceneNameWithoutExtension(EditorBuildSettingsScene scene) =>
            System.IO.Path.GetFileNameWithoutExtension(scene.path);

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