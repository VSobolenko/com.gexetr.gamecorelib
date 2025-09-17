using Game;
using UnityEditor;

namespace GameEditor.BuildTools
{
internal sealed class Builder
{
    private const string BuildPath = "Builds";

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Android APK"),]
    public static void BuildAPK()
    {
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = $"{BuildPath}/game_executor.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None,
        };

        Log.Info("APK build start.");
        PlayerSettings.Android.useCustomKeystore = false;
        Log.Info("Key store disable.");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Log.Info("APK build completed.");
    }

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Android AAB"),]
    public static void BuildAAB()
    {
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = $"{BuildPath}/game_executor.aab",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        Log.Info("AAB build start.");
        PlayerSettings.Android.useCustomKeystore = false;
        Log.Info("Key store disable.");
        EditorUserBuildSettings.buildAppBundle = true;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Log.Info("AAB build completed.");
    }

    [MenuItem(GameData.EditorName + EditorSubfolder.Build + "/Standalone EXE"),]
    public static void BuildStandalone()
    {
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = GetEnabledScenes(),
            locationPathName = $"{BuildPath}/game_executor.exe",
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        Log.Info("PC build start");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Log.Info("PC build completed.");
    }

    private static string[] GetEnabledScenes()
    {
        var scenes = EditorBuildSettings.scenes;
        var enabledScenes = new System.Collections.Generic.List<string>();

        foreach (var scene in scenes)
        {
            if (scene.enabled)
            {
                enabledScenes.Add(scene.path);
            }
        }

        return enabledScenes.ToArray();
    }
    
    public static bool TryGetBuildPathFromArguments(string arguments, out string value)
    {
        value = string.Empty;
        var args = System.Environment.GetCommandLineArgs();
            
        foreach (var arg in args)
        {
            if (arg.StartsWith(arguments) == false) 
                continue;
            
            value = arg.Substring(arguments.Length);
            return true;
        }

        return false;
    }
}
}