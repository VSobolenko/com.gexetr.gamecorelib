using System.Collections.Generic;
using Game;
using Game.Components.Utilities;
using Game.DynamicData;
using UnityEditor;
using UnityEngine;

namespace GameEditor.SceneTools
{
internal class SceneCheckForNullEditorTool : Editor
{
    private static readonly List<string> Accessible = new() {"Assembly-CSharp", "GameCoreLib"};

    [MenuItem(GameData.EditorName + EditorSubfolder.Scene + EditorSubfolder.NullValidator +
              "/Default Validate (Assembly-CSharp)")]
    public static void EditorCheck()
    {
        var monoBehaviours = SceneCheckForNullEditorProvider.GetScripts(Accessible);
        var healthy = true;
        foreach (var monoBehaviour in monoBehaviours)
            healthy &= SceneCheckForNullEditorProvider.CheckField(monoBehaviour, new List<object>(),
                                                                  monoBehaviour.gameObject,
                                                                  monoBehaviour.GetType().Name, false, Accessible);

        if (healthy)
            Log.Info("All objects are healthy");
    }

    [MenuItem(GameData.EditorName + EditorSubfolder.Scene + EditorSubfolder.NullValidator +
              "/Custom Assembly")]
    public static void GameObjectCheck()
    {
        var go = new GameObject("Scene Null Validator", typeof(SceneCheckForNullEditorProvider));
        Undo.RegisterCreatedObjectUndo(go, "Create GameObject");
        Selection.activeGameObject = go;
    }
}
}