using Game.Components.Utilities;
using Game.DynamicData;
using GameEditor.Common;
using UnityEditor;
using UnityEngine;

namespace GameEditor.SceneTools
{
internal sealed class SceneCheckForNullEditorTool : Editor
{
    private static readonly string[] Accessible = { "Assembly-CSharp", "GameCoreLib" };

    [MenuItem(GameData.EditorName + EditorSubfolder.Scene + EditorSubfolder.NullValidator + "/Default Validate (Assembly-CSharp)")]
    public static void EditorCheck() => CheckForNullInspectorEditorProvider.ProcessCheckFieldForNull(Accessible, false);

    [MenuItem(GameData.EditorName + EditorSubfolder.Scene + EditorSubfolder.NullValidator + "/Custom Assembly")]
    public static void GameObjectCheck()
    {
        var go = new GameObject("Scene Null Validator", typeof(CheckForNullEditorProvider));
        Undo.RegisterCreatedObjectUndo(go, "Create GameObject");
        Selection.activeGameObject = go;
    }
}
}