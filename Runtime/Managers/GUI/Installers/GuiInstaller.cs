using System;
using Game.AssetContent;
using Game.AssetContent.Managers;
using Game.GUI.Windows;

namespace Game.GUI.Installers
{
public static partial class GuiInstaller
{
    private const string ResourcesWindowSettingsPath = "UI/WindowSettings";
    private static readonly IResourceManagement ResourceManagement;
    private static WindowSettings _windowSettings;

    static GuiInstaller()
    {
        ResourceManagement = new ResourceManagement();
        _windowSettings = LoadInputSettings();
    }

    private static WindowSettings LoadInputSettings()
    {
        var so = ResourceManagement.LoadAsset<WindowSettingsSo>(ResourcesWindowSettingsPath);

        if (so != null)
            return so.defaultSettings;

        throw new ArgumentNullException(ResourcesWindowSettingsPath, $"Can't load input so settings. Path to so: {ResourcesWindowSettingsPath}");
    }

    public static void SetSettings(WindowSettings windowSettings) => _windowSettings = windowSettings;
}
}
