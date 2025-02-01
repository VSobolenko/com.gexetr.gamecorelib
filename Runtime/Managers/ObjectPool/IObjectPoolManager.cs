using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Pools
{
public interface IObjectPoolManager : IObjectPoolRecyclable
{
    ///  <summary>
    ///  Notify about availability, without preparation or addition
    ///  </summary>
    ///  <param name="prefab">Object prefab or instance</param>
    ///  <param name="count">Expected quantity to add</param>
    ///  <param name="force">force add so much even if already there</param>
    ///  <typeparam name="T">Object type</typeparam>
    IPoolableObjectPool<IPoolable> Prepare<T>(T prefab, int count = 0, bool force = false) where T : Component, IPoolable;

    Task<IPoolableObjectPool<IPoolable>> PrepareAsync<T>(T prefab, int count = 0, bool force = false,
        CancellationToken token = default) where T : Component, IPoolable;

    /// <summary>
    /// Get object instance from pool
    /// </summary>
    /// <param name="prefab">Object prefab or instance</param>
    /// <param name="position">Position for the object</param>
    /// <param name="rotation">Orientation of the object</param>
    /// <param name="parent">Parent that will be assigned to the object</param>
    /// <param name="inWorldSpace">World space</param>
    /// <typeparam name="T">Object type</typeparam>
    /// <returns>The instantiated clone</returns>
    T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true) where T : Component, IPoolable;

    T Get<T>(T prefab) where T : Component, IPoolable;

    T Get<T>(T prefab, Transform parent, bool inWorldSpace = true) where T : Component, IPoolable;

    T Get<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component, IPoolable;
}

public interface IGameObjectObjectPoolManager
{
    IComponentObjectPool<Component> Prepare<T>(T prefab, int count = 0, bool force = false) where T : Component;

    Task<IComponentObjectPool<Component>> PrepareAsync<T>(T prefab, int count = 0, bool force = false,
        CancellationToken token = default) where T : Component;

    T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true) where T : Component;

    T Get<T>(T prefab) where T : Component;

    T Get<T>(T prefab, Transform parent, bool inWorldSpace = true) where T : Component;

    T Get<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component;
    
    void Release<T>(T prefabInstance) where T : Component;
}
}