using System;
using Game.AssetContent;
using Game.AssetContent.Managers;
using Game.GUI.Windows;

namespace Game.GUI.Installers
{
public static partial class GuiInstaller
{
    private const string ResourcesWindowSettingsPath = "UI/WindowSettings";
    private static readonly IResourceManager ResourceManager;
    private static WindowSettings _windowSettings;

    static GuiInstaller()
    {
        ResourceManager = new ResourceManager();
        _windowSettings = LoadInputSettings();
    }

    private static WindowSettings LoadInputSettings()
    {
        var so = ResourceManager.LoadAsset<WindowSettingsSo>(ResourcesWindowSettingsPath);

        if (so != null)
            return so._defaultSettings;

        throw new ArgumentNullException(ResourcesWindowSettingsPath, $"Can't load SO settings. Path to so: {ResourcesWindowSettingsPath}");
    }

    public static void SetSettings(WindowSettings windowSettings) => _windowSettings = windowSettings;
}
}
