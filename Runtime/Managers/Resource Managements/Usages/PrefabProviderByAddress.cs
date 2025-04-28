using System.Threading.Tasks;
using UnityEngine;

namespace Game.AssetContent
{
public abstract class PrefabProviderByAddress<T> where T : class
{
    protected readonly IResourceManager resourceManager;

    private T _cachedPrefab;

    protected PrefabProviderByAddress(IResourceManager resourceManager)
    {
        this.resourceManager = resourceManager;
    }

    protected async Task<T> GetPrefabAsync(string key)
    {
        if (_cachedPrefab != null)
            return _cachedPrefab;

        var prefab = await resourceManager.LoadAssetAsync<GameObject>(key);
        
        if (prefab == null)
        {
            Log.Errored($"Addressable key prefab {key} missing");
            return null;
        }

        var levelSelectionItem = prefab.GetComponent<T>();
        
        if (levelSelectionItem == null)
        {
            Log.Errored($"Component [{typeof(T)}] missing from {prefab.name} gameObject");
            return null;
        }

        _cachedPrefab = levelSelectionItem;
        return levelSelectionItem;
    }
    
    protected T GetPrefab(string key)
    {
        if (_cachedPrefab != null)
            return _cachedPrefab;
        
        var prefab = resourceManager.LoadAsset<GameObject>(key);
        
        if (prefab == null)
        {
            Log.Errored($"Addressable key prefab {key} missing");
            return null;
        }

        var levelSelectionItem = prefab.GetComponent<T>();
        
        if (levelSelectionItem == null)
        {
            Log.Errored($"Component [{typeof(T)}] missing from {prefab.name} gameObject");
            return null;
        }

        return levelSelectionItem;
    }
}

public abstract class ScriptableObjectProviderByAddress<T> where T : ScriptableObject
{
    protected readonly IResourceManager resourceManager;

    private T _cachedPrefab;

    protected ScriptableObjectProviderByAddress(IResourceManager resourceManager)
    {
        this.resourceManager = resourceManager;
    }

    protected async Task<T> GetSOAsync(string key)
    {
        if (_cachedPrefab != null)
            return _cachedPrefab;

        var prefab = await resourceManager.LoadAssetAsync<T>(key);
        
        if (prefab == null)
        {
            Log.Errored($"Addressable key prefab {key} missing");
            return null;
        }

        _cachedPrefab = prefab;
        return prefab;
    }
    
    protected T GetSO(string key)
    {
        if (_cachedPrefab != null)
            return _cachedPrefab;
        
        var prefab = resourceManager.LoadAsset<T>(key);
        
        if (prefab == null)
        {
            Log.Errored($"Addressable key prefab {key} missing");
            return null;
        }

        return prefab;
    }
}
}