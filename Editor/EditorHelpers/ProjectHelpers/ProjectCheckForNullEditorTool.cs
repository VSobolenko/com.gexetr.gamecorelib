using Game;
using Game.Components.Utilities;
using GameEditor.Common;
using UnityEditor;
using UnityEngine;

namespace GameEditor.ProjectTools
{
internal static class ProjectCheckForNullEditorTool
{
    private static readonly string[] Accessible = { "Assembly-CSharp", "GameCoreLib" };

    [MenuItem(GameData.EditorName + EditorSubfolder.Project + EditorSubfolder.NullValidator + "/GameObjects Default Validate (Assembly-CSharp)")]
    private static void VerifyProjectGameObjectsToNull() => VerifyGameObjectsToNull(Accessible);

    private static void VerifyGameObjectsToNull(string[] accessible)
    {
        var guids = AssetDatabase.FindAssets($"t:{nameof(GameObject)}");

        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            var monoBehaviours = CheckForNullInspectorEditorProvider.GetScriptsInRoot(gameObject, accessible);
            CheckForNullInspectorEditorProvider.ProcessCheckFieldForNull(monoBehaviours, accessible, false);
        }
    }

    [MenuItem(GameData.EditorName + EditorSubfolder.Project + EditorSubfolder.NullValidator + "/GameObjects Custom Assembly")]
    public static void VerifyProjectGameObjectsToNullInCustomAssembly()
    {
        var provider = Object.FindFirstObjectByType<CheckForNullEditorProvider>();
        if (provider == null)
        {
            var go = new GameObject("Project Null Validator", typeof(CheckForNullEditorProvider));
            Undo.RegisterCreatedObjectUndo(go, "Create GameObject");
            Selection.activeGameObject = go;

            Log.Info($"\"Custom Assembly\" provider not found! " +
                     $"To use \"Custom Assembly\" firstly configure {go.name} and try again", go);

            return;
        }

        VerifyGameObjectsToNull(provider._accessible);
    }

    [MenuItem(GameData.EditorName + EditorSubfolder.Project + EditorSubfolder.NullValidator + "/Scriptable Objects")]
    private static void VerifyScriptableObjectsToNull()
    {
        var guids = AssetDatabase.FindAssets($"t:{nameof(ScriptableObject)}");

        foreach (var guid in guids)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var scriptableObject = AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetPath);

            var serializedObject = new SerializedObject(scriptableObject);
            var property = serializedObject.GetIterator();
            while (property.NextVisible(true))
                if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
                    Log.Warning($"SO:{scriptableObject.name}; path:{assetPath}; Property={property.name}", scriptableObject);
        }
    }
}
}