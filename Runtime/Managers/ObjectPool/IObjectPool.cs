using System;
using UnityEngine;

namespace Game.Pools
{
    public interface IObjectPool<T> where T : class
    {
        T Get();
        void Release(T instance);
        
        internal int Count { get; }
        protected internal Func<T> CreateInstance { get; }
    }
    
    public interface IGameObjectObjectPool<T> : IObjectPool<T> where T : Component
    {
        T Get(Vector3 position, Quaternion rotation);
        T Get(Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true);
        T Get(Transform parent, bool inWorldSpace = true);
    }

    public interface IPoolableObjectPool<T> : IObjectPool<T> where T : class, IPoolable
    {
        T Get(Vector3 position, Quaternion rotation);
        T Get(Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true);
        T Get(Transform parent, bool inWorldSpace = true);
    }
}