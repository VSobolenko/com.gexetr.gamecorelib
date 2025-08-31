using System;
using Game;
using UnityEngine;

namespace Game.PreferencesSaveType
{
internal sealed class EnumSavableValue<T> : BaseSavableValue<T> where T: struct, Enum
{
    public EnumSavableValue(string playerPrefsKey, T defaultValue = default) : 
        base(playerPrefsKey, defaultValue)
    {
    }

    protected override T LoadValue(ref string path)
    {
        try
        {
            var stringType = PlayerPrefs.GetString(path, defaultValue.ToString());

            return Enum.Parse<T>(stringType);
        }
        catch (Exception e)
        {
            Log.Exception($"Load enum fail: {e.Message}");

            return defaultValue;
        }
    }

    protected override void SaveValue(ref string path) => PlayerPrefs.SetString(path, cachedValue.ToString());
}
}