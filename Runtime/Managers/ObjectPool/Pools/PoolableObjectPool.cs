using System;
using UnityEngine;

namespace Game.Pools
{
public class PoolableObjectPool<T> : ObjectPool<T>, IPoolableObjectPool<T> where T : class, IPoolable
{
    private readonly Transform _root;

    public PoolableObjectPool(int capacity, Transform root, Func<T> createInstance) : base(capacity, createInstance)
    {
        _root = root;
    }

    public override void Release(T instance) => InternalRelease(instance);

    public override T Get() => InternalGet(Vector3.zero, Quaternion.identity, null);

    public T Get(Vector3 position, Quaternion rotation) => InternalGet(position, rotation, null);

    public T Get(Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true) =>
        InternalGet(position, rotation, parent, inWorldSpace);

    public T Get(Transform parent, bool inWorldSpace = true) =>
        InternalGet(Vector3.zero, Quaternion.identity, parent, inWorldSpace);

    private T InternalGet(Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true)
    {
        var instance = Pool.Count != 0 ? Pool.Dequeue() : CreateInstance();

        if (inWorldSpace)
        {
            instance.SetPositionAndRotation(position, rotation);
            instance.SetParent(parent);
        }
        else
        {
            instance.SetParent(parent);
            instance.SetPositionAndRotation(position, rotation);
        }

        instance.SetActive(true);
        instance.OnUse();

        return instance;
    }

    private void InternalRelease(T instance)
    {
        if (instance == null)
            throw new InvalidOperationException($"The element \"{typeof(T).Name}\" is null!");
        if (Pool.Contains(instance))
            throw new InvalidOperationException($"The element \"{instance.GetType().Name}\" is already in the pool!");

        Pool.Enqueue(instance);
        instance.SetActive(false);
        instance.SetParent(_root);
        instance.OnRelease();
    }
}
}