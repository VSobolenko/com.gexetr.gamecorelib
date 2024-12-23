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
    /// void Warn<T>(T prefab, int count) where T : Object, IPoolable;
    ///  <summary>
    ///  Add ready instances to pool
    ///  </summary>
    ///  <param name="prefab">Object prefab or instance</param>
    ///  <param name="count">Quantity to add</param>
    ///  <typeparam name="T">Object type</typeparam>
    void Prepare<T>(T prefab, int count = 0, bool force = false) where T : Component, IPoolable;

    Task PrepareAsync<T>(T prefab, int count = 0, bool force = false, CancellationToken token = default) where T : Component, IPoolable;

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

public interface IPrefabObjectPool<T> where T : Component, IPoolable
{
    T Get();
    T Get(Vector3 position, Quaternion rotation);
    T Get(Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true);
    T Get(Transform parent, bool inWorldSpace = true);
    void Release(T prefabInstance);
}
}