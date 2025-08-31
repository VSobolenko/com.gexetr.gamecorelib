using System;
using Game.Factories;
using UnityEngine;

namespace Game.AssetContent.Managers
{
internal class ResourceManagerFactory : IResourceFactoryManager
{
    private readonly IResourceManager _resourceManager;
    private readonly IFactoryGameObjects _factoryGameObjects;

    public ResourceManagerFactory(IResourceManager resourceManager, IFactoryGameObjects factoryGameObjects)
    {
        _resourceManager = resourceManager;
        _factoryGameObjects = factoryGameObjects;
    }

    public T CreateGameObjectWithComponent<T>(string key, Vector3 position, Quaternion quaternion, Transform parent)
        where T : Component
    {
        var prefab = LoadPrefabMonoBeh<T>(key);
        var instance = _factoryGameObjects.Instantiate(prefab, position, quaternion, parent);
        return instance;
    }

    private T LoadPrefabMonoBeh<T>(string key) where T : Component
    {
        var prefab = _resourceManager.LoadAsset<GameObject>(key);

        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab), $"Addressable key prefab {key} missing");

        var monoBeh = prefab.GetComponent<T>();

        if (monoBeh == null)
            throw new ArgumentNullException(nameof(prefab), $"Component [{typeof(T)}] missing from {prefab.name} gameObject");

        return monoBeh;
    }
}
}