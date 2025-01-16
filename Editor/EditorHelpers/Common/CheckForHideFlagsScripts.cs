using System.Collections.Generic;
using System.Linq;
using Game;
using Game.DynamicData;
using GameEditor.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameEditor.Common
{
internal static class CheckForHighFlagsScripts
{
    [MenuItem(GameData.EditorName + EditorSubfolder.Scene + "/Find Unpredictable Objects")]
    private static void FindUnpredictableObjectsInScene()
    {
        var isFounded = false;
        foreach (var scene in GetScenes())
        {
            var gameObjects = scene.GetRootGameObjects();

            foreach (var gameObject in gameObjects)
                isFounded |= ContainsUnpredictableObjectsInRoot(gameObject, scene.name);
        }

        if (InternalTools.IsPrefabStage(out var prefabRoot))
            isFounded |= ContainsUnpredictableObjectsInRoot(prefabRoot.gameObject, SceneManager.GetActiveScene().name);
        
        if (isFounded == false)
            Log.Info("All objects are clean!");
    }

    private static Scene[] GetScenes()
    {
        var scenes = new HashSet<Scene> {SceneManager.GetActiveScene()};
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
            Log.Warning(
                $"\"{root.name}\" unpredictable behavior is possible. Flag={root.hideFlags}\n Path: {sceneName}/{InternalTools.GetObjectHierarchyPath(root)}",
                root);
        }

        foreach (Transform child in root.transform)
            isFounded |= ContainsUnpredictableObjectsInRoot(child.gameObject, sceneName);

        return isFounded;
    }
}
}