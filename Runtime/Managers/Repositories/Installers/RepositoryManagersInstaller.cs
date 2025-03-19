using System;
using Game.IO;
using Game.PreferencesSaveType;
using Game.Repositories.Managers;

namespace Game.Repositories
{
public static class RepositoryInstaller
{
    public static IRepository<T> File<T>(string path, ISaveFile fileSaver) where T : class, IHasBasicId =>
        new FileRepositoryManager<T>(path, fileSaver);

    public static IRepository<T> StaticResources<T>(string path, ISaveFile fileSaver) where T : class, IHasBasicId =>
        new StaticResourcesRepositoryManager<T>(path, fileSaver);
    
    public static BoolSavableValue Bool(string key, bool defaultValue) => new BoolSavableValue(key, defaultValue);
    
    public static FloatSavableValue Float(string key, float defaultValue) => new FloatSavableValue(key, defaultValue);
    
    public static IntSavableValue Float(string key, int defaultValue) => new IntSavableValue(key, defaultValue);

    public static StringSavableValue String(string key, string defaultValue) =>
        new StringSavableValue(key, defaultValue);

    public static object Enum<T>(string key, T defaultValue) where T : struct, Enum =>
        new EnumSavableValue<T>(key, defaultValue);

    public static SerializableSavableValue<T> Value<T>(string key, T defaultValue) =>
        new SerializableSavableValue<T>(key, defaultValue);
}
}