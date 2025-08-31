using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Game.PreferencesSaveType
{
public sealed class SerializableSavableValue<T> : BaseSavableValue<T>
{
    public SerializableSavableValue(string playerPrefsKey, T defaultValue = default) 
        : base(playerPrefsKey, defaultValue)
    {
    }

    //ToDo: refactor(using(...)) ant testing this elements
    protected override T LoadValue(ref string path)
    {
        Log.Errored("NE DODELANO");
        var stringToDeserialize = PlayerPrefs.GetString(path, "");

        var bytes = Convert.FromBase64String(stringToDeserialize);
        var memoryStream = new MemoryStream(bytes);
        var bf = new BinaryFormatter();

        return (T)bf.Deserialize(memoryStream);
    }

    //ToDo: refactor(using(...)) ant testing this elements
    protected override void SaveValue(ref string path)
    {
        Log.Errored("NE DODELANO");
        var memoryStream = new MemoryStream();
        var bf = new BinaryFormatter();
        bf.Serialize(memoryStream, cachedValue);
        var stringToSave = Convert.ToBase64String(memoryStream.ToArray());

        PlayerPrefs.SetString(path, stringToSave);
    }
}
}