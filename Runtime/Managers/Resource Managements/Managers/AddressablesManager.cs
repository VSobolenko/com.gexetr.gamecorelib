using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Game.AssetContent.Managers
{
internal sealed class AddressablesManager : System.IDisposable, IResourceManager
{
    private readonly Dictionary<string, AsyncOperationHandle> _loadedHandlers;

    public AddressablesManager()
    {
        _loadedHandlers = new Dictionary<string, AsyncOperationHandle>();

        Addressables.InitializeAsync();
    }

    public T LoadAsset<T>(string key) where T : Object
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Unable to load null or empty key");

        if (_loadedHandlers.TryGetValue(key, out var handler))
        {
            var existHandle = handler.Convert<T>();

            return existHandle.IsDone == false ? existHandle.WaitForCompletion() : existHandle.Result;
        }

        if (Application.isEditor && IsKeyExist(key) == false)
            throw new ArgumentNullException(key, $"Asset key not found: {key}");

        if (IsKeyExist(key) == false)
            return null;

        var handle = Addressables.LoadAssetAsync<T>(key);
        var asset = handle.WaitForCompletion();

        if (IsHandleCompleteSuccess(ref handle) == false)
        {
            Log.Errored($"Asset loading error: Status={handle.Status}; IsDone={handle.IsDone}; Key={key}");

            return default;
        }
        
        _loadedHandlers.Add(key, handle);

        return asset;
    }

    public async Task<T> LoadAssetAsync<T>(string key) where T : Object
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Unable to load null or empty key");

        if (_loadedHandlers.ContainsKey(key))
        {
            var existHandle = _loadedHandlers[key].Convert<T>();

            return existHandle.IsDone == false ? existHandle.WaitForCompletion() : existHandle.Result;
        }

        if (await IsKeyExistAsync(key) == false)
        {
            Log.Warning($"Asset key not found: \"{key}\"");

            return null;
        }

        var handle = Addressables.LoadAssetAsync<T>(key);
        await handle.Task;

        if (IsHandleCompleteSuccess(ref handle) == false)
        {
            Log.Errored($"Asset loading error: Status={handle.Status}; IsDone={handle.IsDone}");

            return default;
        }

        _loadedHandlers.Add(key, handle);

        return handle.Result;
    }

    public async Task LoadSceneAsync(string key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("Unable to load empty key");

        if (await IsKeyExistAsync(key) == false)
            throw new ArgumentException("Asset key not found: \"{key}\"");

        var handle = Addressables.LoadSceneAsync(key, loadMode, activateOnLoad);

        await handle.Task;
    }

    // ToDo: test this method
    public void ClearMemory()
    {
        foreach (var (key, handler) in _loadedHandlers)
        {
            if (handler.IsValid())
                Addressables.Release(handler);
            else
                Log.Errored($"handler in not valid: {key}");
        }

        _loadedHandlers.Clear();
    }

    // ToDo: what is it
    public void Dispose()
    {
    }

    private static async Task<bool> IsKeyExistAsync(string key)
    {
        var handle = Addressables.LoadResourceLocationsAsync(key);
        var result = await handle.Task;

        if (IsHandleCompleteSuccess(ref handle) == false)
        {
            Log.Errored($"Asset loading error: Status={handle.Status}; IsDone={handle.IsDone}");

            return default;
        }

        return result.Count > 0;
    }

    private static bool IsKeyExist(string key)
    {
        var handle = Addressables.LoadResourceLocationsAsync(key);
        var result = handle.WaitForCompletion();

        if (IsHandleCompleteSuccess(ref handle) == false)
        {
            Log.Errored($"Asset loading error: Status={handle.Status}; IsDone={handle.IsDone}");

            return default;
        }

        return result.Count > 0;
    }
    
    private static bool IsHandleCompleteSuccess<T>(ref AsyncOperationHandle<T> handle) => 
        handle.Status == AsyncOperationStatus.Succeeded && handle.IsDone && handle.IsValid();
}
}