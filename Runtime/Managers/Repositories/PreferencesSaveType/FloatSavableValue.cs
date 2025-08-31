using UnityEngine;

namespace Game.PreferencesSaveType
{
public sealed class FloatSavableValue : BaseSavableValue<float>
{
    public FloatSavableValue(string playerPrefsKey, float defaultValue = default) 
        : base(playerPrefsKey, defaultValue)
    {
    }

    protected override float LoadValue(ref string path) => PlayerPrefs.GetFloat(path, defaultValue);

    protected override void SaveValue(ref string path) => PlayerPrefs.SetFloat(path, cachedValue);
}
}