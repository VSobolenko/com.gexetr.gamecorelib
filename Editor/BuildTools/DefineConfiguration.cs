using System.Collections.Generic;
using System.Linq;
using Game.DynamicData;
using UnityEditor;

namespace GameEditor.BuildTools
{
internal class DefineConfiguration
{
    private const string LogDefine = "ENABLE_LOG";
    private const string DevBuildDefine = "DEVELOPMENT_BUILD";
    private const string ReleaseBuildDefine = "RELEASE_BUILD";

    [MenuItem(GameData.EditorName + "/Prepare build/Full Debug"),]
    public static void FullDebug()
    {
        SetDefine(LogDefine, DevBuildDefine);
        RemoveDefine(ReleaseBuildDefine);
    }
    
    [MenuItem(GameData.EditorName + "/Prepare build/Full Release"),]
    public static void FullRelease()
    {
        SetDefine(ReleaseBuildDefine);
        RemoveDefine(LogDefine, DevBuildDefine);
    }

    [MenuItem(GameData.EditorName + "/Prepare build/Add/Log"),]
    public static void LogEnable() => SetDefine(LogDefine);

    [MenuItem(GameData.EditorName + "/Prepare build/Add/Release"),]
    public static void ReleaseEnable() => SetDefine(ReleaseBuildDefine);

    [MenuItem(GameData.EditorName + "/Prepare build/Add/Build"),]
    public static void DebugEnable() => SetDefine(DevBuildDefine);

    [MenuItem(GameData.EditorName + "/Prepare build/Add/Build And Log"),]
    public static void DebugAndLogEnable() => SetDefine(DevBuildDefine, LogDefine);

    [MenuItem(GameData.EditorName + "/Prepare build/Add/Release And Log"),]
    public static void ReleaseAndLogEnable() => SetDefine(ReleaseBuildDefine, LogDefine);
    

    [MenuItem(GameData.EditorName + "/Prepare build/Remove/Log"),]
    public static void LogDisable() => RemoveDefine(LogDefine);

    [MenuItem(GameData.EditorName + "/Prepare build/Remove/Release"),]
    public static void ReleaseDisable() => RemoveDefine(ReleaseBuildDefine);

    [MenuItem(GameData.EditorName + "/Prepare build/Remove/Build"),]
    public static void DebugDisable() => RemoveDefine(DevBuildDefine);

    [MenuItem(GameData.EditorName + "/Prepare build/Remove/Build And Log"),]
    public static void DebugAndLogDisable() => RemoveDefine(DevBuildDefine, LogDefine);

    [MenuItem(GameData.EditorName + "/Prepare build/Remove/Release And Log"),]
    public static void ReleaseAndLogDisable() => RemoveDefine(ReleaseBuildDefine, LogDefine);

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