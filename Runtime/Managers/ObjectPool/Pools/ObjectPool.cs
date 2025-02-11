using System;
using System.Collections.Generic;

namespace Game.Pools
{
public class ObjectPool<T> : IObjectPool<T> where T : class
{
    protected readonly Queue<T> Pool;
    protected readonly Func<T> CreateInstance;

    int IObjectPool<T>.Count => Pool.Count;
    Func<T> IObjectPool<T>.CreateInstance => CreateInstance;

    protected ObjectPool(int capacity, Func<T> createInstance)
    {
        CreateInstance = createInstance;
        Pool = new Queue<T>(capacity);
    }

    public virtual T Get() => Pool.Count > 0 ? Pool.Dequeue() : CreateInstance();

    public virtual void Release(T instance)
    {
        if (Pool.Contains(instance))
            throw new InvalidOperationException($"The element \"{instance.GetType().Name}\" is already in the pool!");
        Pool.Enqueue(instance);
    }
}
}