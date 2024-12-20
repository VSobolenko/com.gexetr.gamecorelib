using UnityEngine;

namespace Game.PreferencesSaveType
{
public class StringSavableValue : BaseSavableValue<string>
{
    public StringSavableValue(string playerPrefsKey, string defaultValue = default) 
        : base(playerPrefsKey, defaultValue)
    {
    }

    protected override string LoadValue(ref string path) => PlayerPrefs.GetString(path, defaultValue);

    protected override void SaveValue(ref string path) => PlayerPrefs.SetString(path, cachedValue);
}
}