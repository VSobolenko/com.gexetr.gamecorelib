using Game;
using Game.Components;
using Game.DynamicData;
using Game.Extensions;
using GameEditor.Internal;
using UnityEngine;

namespace GameEditor.ProjectTools
{
internal static class InGameDebugConsoleProviderEditorTool
{
    private const string InResourcesFolder = "Assets/Plugins/IngameDebugConsole/Resources/IngameDebugConsole.prefab";
    private const string OutsideResourcesFolder = "Assets/Plugins/IngameDebugConsole/IngameDebugConsole.prefab";

    private const string Title = "In-game console";

    private const string InspectAsset = "Inspect prefab asset";
    private const string ToResources = "Move to resource folder";
    private const string FromResources = "Move from resource folder";
    private const string CreateSceneProvider = "Create Scene GameObject";

    [UnityEditor.MenuItem(GameData.EditorName + EditorSubfolder.Project + "/" + Title + "/" + CreateSceneProvider)]
    public static void CreateSceneGameObjectStatic()
    {
        if (InternalTools.IsPrefabStage(out var root)) { }

        var provider = new GameObject()
            .With(x => x.transform.SetParent(root))
            .With(x => x.name = "In Game Console Provider")
            .AddComponent<InGameDebugConsoleProvider>();
        
        UnityEditor.Selection.activeGameObject = provider.gameObject;
    }
    
    [UnityEditor.MenuItem(GameData.EditorName + EditorSubfolder.Project + "/" + Title + "/" + InspectAsset)]
    public static void SelectInGamePrefabStatic()
    {
        var inGameConsole = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(InResourcesFolder);
        if (inGameConsole == null)
            inGameConsole = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(OutsideResourcesFolder);

        if (inGameConsole == null)
        {
            Log.Warning("Can't find asset");

            return;
        }

        UnityEditor.Selection.activeObject = inGameConsole;
    }

    [UnityEditor.MenuItem(GameData.EditorName + EditorSubfolder.Project + "/" + Title + "/" + ToResources)]
    public static void MoveToResourcesStatic()
    {
        var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(OutsideResourcesFolder);
        if (asset == null)
        {
            Log.Warning($"Asset from [{OutsideResourcesFolder}] not found");

            return;
        }

        var result = UnityEditor.AssetDatabase.MoveAsset(OutsideResourcesFolder, InResourcesFolder);
        Log.Info($"Result moving: {(string.IsNullOrEmpty(result) ? "Success" : result)}");

        UnityEditor.AssetDatabase.Refresh();
    }

    [UnityEditor.MenuItem(GameData.EditorName + EditorSubfolder.Project + "/" + Title + "/" + FromResources)]
    public static void MoveFromResourcesStatic()
    {
        var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(InResourcesFolder);
        if (asset == null)
        {
            Log.Warning($"Asset from [{InResourcesFolder}] not found");

            return;
        }

        var result = UnityEditor.AssetDatabase.MoveAsset(InResourcesFolder, OutsideResourcesFolder);
        Log.Info($"Result moving: {(string.IsNullOrEmpty(result) ? "Success" : result)}");

        UnityEditor.AssetDatabase.Refresh();
    }
}
}