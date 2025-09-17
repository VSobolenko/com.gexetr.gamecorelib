using System.Collections.Generic;
using System.Linq;
using Game;
using GameEditor.Internal;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameEditor.Common
{
internal static class CheckForMissingScripts
{
    [MenuItem(GameData.EditorName + EditorSubfolder.Scene + "/Find Missing Scripts in Build Scenes")]
    private static void FindMissingScriptsInScene()
    {
        var isFounded = false;
        foreach (var scene in GetScenes())
        {
            var gameObjects = scene.GetRootGameObjects();

            foreach (var gameObject in gameObjects)
                isFounded |= ContainsMissingScriptsInRoot(gameObject, scene.name);
        }

        if (InternalTools.IsPrefabStage(out var prefabRoot))
            isFounded |= ContainsMissingScriptsInRoot(prefabRoot.gameObject, SceneManager.GetActiveScene().name);

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

    private static bool ContainsMissingScriptsInRoot(GameObject root, string sceneName)
    {
        var isFounded = false;
        var components = root.GetComponents<Component>();
        foreach (var component in components)
        {
            if (component != null)
                continue;

            Log.Warning($"\"{root.name}\" contains missing script\n Path: {sceneName}/{InternalTools.GetObjectHierarchyPath(root)}", root);
            isFounded = true;
        }

        foreach (Transform child in root.transform)
            isFounded |= ContainsMissingScriptsInRoot(child.gameObject, sceneName);

        return isFounded;
    }
}
}