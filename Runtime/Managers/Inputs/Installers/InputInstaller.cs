using System;
using Game.AssetContent.Managers;
using Game.Inputs.Managers;
using UnityEngine;

namespace Game.Inputs.Installers
{
public static class InputInstaller
{
    private const string ResourcesSettingsPath = "InputSettings";
    private static InputSettings _settings;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatic()
    {
        _settings = null;
    }
    
    public static IInputManager Manager() => new InputManager();
    
    public static SwipeDetector Swipe(IInputManager manager) => new SwipeDetector(manager, _settings);
    
    public static InputSettings LoadDefaultSettingsFromResources()
    {
        var resourceManager = new ResourceManager();
        var so = resourceManager.LoadAsset<InputSettingsSo>(ResourcesSettingsPath);

        if (so == null) 
            throw new ArgumentNullException(ResourcesSettingsPath, $"Can't load SO settings. Path to so: {ResourcesSettingsPath}");
        
        _settings = so.inputSettings;
        return so.inputSettings;
    }
}
}