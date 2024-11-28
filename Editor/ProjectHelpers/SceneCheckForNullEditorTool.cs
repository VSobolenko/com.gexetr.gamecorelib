using System.Collections.Generic;
using Game;
using Game.Components.Utilities;
using Game.DynamicData;
using UnityEditor;
using UnityEngine;

namespace GameEditor.Tools
{
public class SceneCheckForNullEditorTool : Editor
{
    private const string Subdirectory = "/Scene Null Validator";
    private static readonly List<string> Accessible = new() {"Assembly-CSharp", "GameCoreLib"};

    [MenuItem(GameData.EditorName + Subdirectory + "/Default Validate (Assembly-CSharp)")]
    public static void EditorCheck()
    {
        var scripts = SceneCheckForNullEditorProvider.GetScripts(Accessible);
        var healthy = true;
        foreach (var script in scripts)
            healthy &= SceneCheckForNullEditorProvider.CheckField(script, new List<object>(), script.gameObject,
                                                              script.GetType().Name, false, Accessible);
        
        if (healthy)
            Log.Info("All objects are healthy");
    }

    [MenuItem(GameData.EditorName + Subdirectory + "/Custom Assembly")]
    public static void GameObjectCheck()
    {
        var go = new GameObject("Scene Null Validator", typeof(SceneCheckForNullEditorProvider));
        Selection.activeGameObject = go;
    }
}
}