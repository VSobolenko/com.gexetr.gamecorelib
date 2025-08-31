using System;
using Game.AssetContent;
using Game.AssetContent.Managers;
using Game.GUI.Windows;
using Game.GUI.Windows.Factories;
using UnityEngine;

namespace Game.GUI
{
public static partial class GuiInstaller
{
    private const string ResourcesSettingsPath = "UI/WindowSettings";
    private static WindowSettings _settings;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatic()
    {
        _settings = null;
    }

    public static IMediatorInstantiator DefaultMediatorInstantiator() => new DefaultMediatorInstantiator();

    public static WindowSettings LoadDefaultSettingsFromResources()
    {
        var resourceManager = new ResourceManager();
        var so = resourceManager.LoadAsset<WindowSettingsSo>(ResourcesSettingsPath);

        if (so == null)
            throw new ArgumentNullException(ResourcesSettingsPath, $"Can't load SO settings. Path to so: {ResourcesSettingsPath}");

        _settings = so._defaultSettings;
        return so._defaultSettings;
    }

    public static void SetSettings(WindowSettings windowSettings) => _settings = windowSettings;
}
}
