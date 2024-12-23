using System.Collections.Generic;
using System.Linq;
using Game;
using Game.DynamicData;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameEditor.Common
{
    public static class CheckForMissingScripts
    {
        [MenuItem(GameData.EditorName + EditorToolsSubfolder.Scene + "/Find Missing Scripts")]
        private static void FindMissingScriptsInScene()
        {
            var isFounded = false;
            foreach (var scene in GetScenes())
            {
                var gameObjects = scene.GetRootGameObjects();

                foreach (var gameObject in gameObjects)
                    isFounded |= ContainsMissingScriptsInRoot(gameObject, scene.name);
            }

            if (isFounded == false)
                Log.Info("All objects are clean!");
        }

        private static Scene[] GetScenes()
        {
            var scenes = new HashSet<Scene> { SceneManager.GetActiveScene() };
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                scenes.Add(scene);
            }

            return scenes.ToArray();
        }

        private static bool ContainsMissingScriptsInRoot(GameObject root, string sceneName)
        {
            var isFounded = false;
            var components = root.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component != null)
                    continue;

                Log.Warning($"\"{root.name}\" contains missing script\n Path: {sceneName}/{GetObjectPath(root)}", root);
                isFounded = true;
            }

            foreach (Transform child in root.transform)
                isFounded |= ContainsMissingScriptsInRoot(child.gameObject, sceneName);

            return isFounded;
        }
        
        private static string GetObjectPath(Transform t) =>
            t.parent == null ? t.name : $"{GetObjectPath(t.parent)}/{t.name}";

        private static string GetObjectPath(GameObject g) =>
            g.transform.parent == null ? g.name : $"{GetObjectPath(g.transform.parent)}/{g.name}";
    }
}