using System;
using Game.Localizations.Components;
using Game.Localizations.Managers;
using UnityEngine;

namespace Game.Localizations.Installers
{
public static class LocalizationInstaller
{
    private const string ResourcesSettingsPath = "Localization/LocalizationSettings";
    internal static LocalizationSettings settings;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticValues()
    {
        settings = null;
    }
    
    public static ILocalizationManager Manager() => new LocalizationManager(settings);
    
    public static void LoadDefaultSettingsFromResources()
    {
        var so = Resources.Load<LocalizationSettings>(ResourcesSettingsPath);
        
        if (so == null)
            throw new ArgumentNullException(ResourcesSettingsPath, $"Can't load SO settings. Path to so: {ResourcesSettingsPath}");
        
        settings = so;
    }
}
}