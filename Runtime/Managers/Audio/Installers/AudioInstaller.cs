using System;
using Game.AssetContent.Managers;
using Game.Audio.Managers;
using Game.Factories;
using UnityEngine;

namespace Game.Audio
{
public static class AudioInstaller
{
    private static AudioSettings _settings;
    private const string ResourcesSettingsPath = "Audio/AudioSettings";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatic()
    {
        _settings = null;
    }

    public static IAudioManager UnityAudio(IFactoryGameObjects factory) => new UnityAudioManager(factory, _settings);
    
    public static void LoadDefaultSettingsFromResources()
    {
        var resourceManager = new ResourceManager();
        var so = resourceManager.LoadAsset<AudioSettings>(ResourcesSettingsPath);
        if (so == null)
            throw new ArgumentNullException(ResourcesSettingsPath, $"Can't load SO settings. Path to so: {ResourcesSettingsPath}");

        _settings = so;
    }
}
}