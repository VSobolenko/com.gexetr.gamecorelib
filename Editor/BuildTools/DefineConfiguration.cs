using System.Collections.Generic;
using System.Linq;
using Game.DynamicData;
using UnityEditor;

namespace GameEditor.BuildTools
{
internal static class DefineConfiguration
{
    private const string LogDisableDefine = "DISABLE_LOG";
    private const string DevBuildDefine = "DEVELOPMENT_BUILD";
    private const string ReleaseBuildDefine = "RELEASE_BUILD";

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Prepare/Full Debug"),]
    public static void FullDebug()
    {
        SetDefine(DevBuildDefine);
        RemoveDefine(ReleaseBuildDefine, LogDisableDefine);
    }
    
    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Prepare/Full Release"),]
    public static void FullRelease()
    {
        SetDefine(ReleaseBuildDefine, LogDisableDefine);
        RemoveDefine(DevBuildDefine);
    }

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Prepare/Add/Log"),]
    public static void LogEnable() => RemoveDefine(LogDisableDefine);

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Prepare/Add/Release"),]
    public static void ReleaseEnable() => SetDefine(ReleaseBuildDefine);

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Prepare/Add/Build"),]
    public static void DebugEnable() => SetDefine(DevBuildDefine);

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Prepare/Add/Build And Log"),]
    public static void DebugAndLogEnable() => SetAndRemoveDefine(DevBuildDefine, LogDisableDefine);

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Prepare/Add/Release And Log"),]
    public static void ReleaseAndLogEnable() => SetAndRemoveDefine(ReleaseBuildDefine, LogDisableDefine);
    

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Prepare/Remove/Log"),]
    public static void LogDisable() => SetDefine(LogDisableDefine);

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Prepare/Remove/Release"),]
    public static void ReleaseDisable() => RemoveDefine(ReleaseBuildDefine);

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Prepare/Remove/Build"),]
    public static void DebugDisable() => RemoveDefine(DevBuildDefine);

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Prepare/Remove/Build And Log"),]
    public static void DebugAndLogDisable() => RemoveAndSetDefine(DevBuildDefine, LogDisableDefine);

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Prepare/Remove/Release And Log"),]
    public static void ReleaseAndLogDisable() => RemoveAndSetDefine(ReleaseBuildDefine, LogDisableDefine);

    private static void SetAndRemoveDefine(string set, string remove)
    {
        SetDefine(set);
        RemoveDefine(remove);
    }
    
    private static void RemoveAndSetDefine(string remove, string set)
    {
        SetDefine(set);
        RemoveDefine(remove);
    }
    
    private static void SetDefine(params string[] defines)
    {
        var activeDefines = GenerateDefineList(EditorUserBuildSettings.selectedBuildTargetGroup);
        var startedLength = activeDefines.Count;

        foreach (var define in defines)
        {
            if (activeDefines.Contains(define))
                continue;
            activeDefines.Add(define);
        }

        if (startedLength == activeDefines.Count)
            return;

        SetDefineList(EditorUserBuildSettings.selectedBuildTargetGroup, activeDefines);
    }

    private static void RemoveDefine(params string[] defines)
    {
        var activeDefines = GenerateDefineList(EditorUserBuildSettings.selectedBuildTargetGroup);
        var startedLength = activeDefines.Count;
        foreach (var define in defines)
            activeDefines.Remove(define);

        if (startedLength == activeDefines.Count)
            return;

        SetDefineList(EditorUserBuildSettings.selectedBuildTargetGroup, activeDefines);
    }

    private static List<string> GenerateDefineList(BuildTargetGroup buildTargetGroup)
    {
        var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        return definesString.Split(';').ToList();
    }

    private static void SetDefineList(BuildTargetGroup buildTargetGroup, List<string> allDefines)
    {
        var definesString = string.Join(";", allDefines.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, definesString);
    }
}
}