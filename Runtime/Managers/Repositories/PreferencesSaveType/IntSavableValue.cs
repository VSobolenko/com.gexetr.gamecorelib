using UnityEngine;

namespace Game.PreferencesSaveType
{
public class IntSavableValue : BaseSavableValue<int>
{
    public IntSavableValue(string playerPrefsKey, int defaultValue = default) 
        : base(playerPrefsKey, defaultValue)
    {
    }

    protected override int LoadValue(ref string path) => PlayerPrefs.GetInt(path, defaultValue);

    protected override void SaveValue(ref string path) => PlayerPrefs.SetInt(path, cachedValue);
}
}