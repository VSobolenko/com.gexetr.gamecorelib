using Game.AssetContent;
using Game.AssetContent.Managers;
using Game.GUI.Windows;
using Game.Utility;

namespace Game.GUI.Installers
{
public static partial class GuiInstaller
{
    private const string ResourcesWindowSettingsPath = "WindowSettings";
    private static readonly WindowSettings WindowSettings;
    private static readonly IResourceManagement Resource;

    static GuiInstaller()
    {
        Resource = new ResourceManagement();
        WindowSettings = LoadInputSettings();
    }

    private static WindowSettings LoadInputSettings()
    {
        var so = Resource.LoadAsset<WindowSettingsSo>(ResourcesWindowSettingsPath);

        if (so != null)
            return so.defaultSettings;
        Log.Error($"Can't load input so settings. Path to so: {ResourcesWindowSettingsPath}");

        return default;
    }
}
}
