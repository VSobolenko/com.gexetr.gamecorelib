using System.Collections.Generic;
using System.Linq;
using Game;
using Game.DynamicData;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameEditor.Common
{
    public static class CheckForHighFlagsScripts
    {
        [MenuItem(GameData.EditorName + EditorToolsSubfolder.Scene + "/Find Unpredictable Objects")]
        private static void FindUnpredictableObjectsInScene()
        {
            var isFounded = false;
            foreach (var scene in GetScenes())
            {
                var gameObjects = scene.GetRootGameObjects();

                foreach (var gameObject in gameObjects)
                    isFounded |= ContainsUnpredictableObjectsInRoot(gameObject, scene.name);
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
        
        private static bool ContainsUnpredictableObjectsInRoot(GameObject root, string sceneName)
        {
            var isFounded = false;
            if (root.hideFlags != HideFlags.None)
            {
                isFounded = true;
                Log.Warning($"\"{root.name}\" unpredictable behavior is possible. Flag={root.hideFlags}\n Path: {sceneName}/{GetObjectPath(root)}", root);
            }
            
            foreach (Transform child in root.transform)
                isFounded |= ContainsUnpredictableObjectsInRoot(child.gameObject, sceneName);

            return isFounded;
        }

        private static string GetObjectPath(Transform t) =>
            t.parent == null ? t.name : $"{GetObjectPath(t.parent)}/{t.name}";

        private static string GetObjectPath(GameObject g) =>
            g.transform.parent == null ? g.name : $"{GetObjectPath(g.transform.parent)}/{g.name}";
    }
}