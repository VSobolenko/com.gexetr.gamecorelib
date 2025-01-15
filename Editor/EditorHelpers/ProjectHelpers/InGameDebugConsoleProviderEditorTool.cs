using Game;
using Game.DynamicData;
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
    
    [UnityEditor.MenuItem(GameData.EditorName + EditorToolsSubfolder.Project + "/" + Title + "/" + InspectAsset)]
    private static void SelectInGamePrefabStatic()
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
    
    [UnityEditor.MenuItem(GameData.EditorName + EditorToolsSubfolder.Project + "/" + Title + "/" + ToResources)]
    private static void MoveToResourcesStatic()
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
    
    [UnityEditor.MenuItem(GameData.EditorName + EditorToolsSubfolder.Project + "/" + Title + "/" + FromResources)]
    private static void MoveFromResourcesStatic()
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