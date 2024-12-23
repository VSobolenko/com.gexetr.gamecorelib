using System;
using System.Collections.Generic;
using Game.Factories;
using UnityEngine;

namespace Game.Pools.Managers
{
    internal class KeyObjectPool<T> : IPrefabObjectPool<T> where T : Component, IPoolable
    {
        private readonly T _prefab;
        private readonly IFactoryGameObjects _factoryGameObjects;
        private readonly Stack<IPoolable> _pool = new();
        private readonly Transform _root;
        private readonly Transform _rootUi;
        private IObjectPoolRecyclable _recyclableManager;

        public KeyObjectPool(T prefab)
        {
            _prefab = prefab;
            if (_prefab == null)
                throw new ArgumentException($"Can't get null prefab");
        }

        public T Get() => InternalGet(Vector3.zero, Quaternion.identity, null);

        public T Get(Vector3 position, Quaternion rotation) => InternalGet(position, rotation, null);

        public T Get(Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true) =>
            InternalGet(position, rotation, parent, inWorldSpace);

        public T Get(Transform parent, bool inWorldSpace = true) =>
            InternalGet(Vector3.zero, Quaternion.identity, parent, inWorldSpace);
        
        public void Release(T prefabInstance)
        {
            if (prefabInstance == null)
                throw new ArgumentException($"Can't release null prefab");

            CreateOrReturnElementToPool(prefabInstance, true);
        }
        
        private T InternalGet(Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true)
        {
            if (_pool.Count == 0)
                CreateOrReturnElementToPool(_prefab, false);
        
            var pooledObject = _pool.Pop();
            if (inWorldSpace)
            {
                pooledObject.SetPositionAndRotation(position, rotation);
                pooledObject.SetParent(parent);
            }
            else
            {
                pooledObject.SetParent(parent);
                pooledObject.SetPositionAndRotation(position, rotation);
            }
            pooledObject.SetActive(true);
            pooledObject.OnUse();

            return (T) pooledObject;
        }
        
        public void CreateOrReturnElementToPool(T prefab, bool isInstance)
        {
            var parent = GetPoolRoot(prefab);
            var pooledObject = isInstance ? prefab : _factoryGameObjects.Instantiate(prefab, parent);

            _pool.Push(pooledObject);
            if (isInstance)
            {
                pooledObject.SetParent(parent);
                pooledObject.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            }

            pooledObject.SetActive(false);
            pooledObject.OnRelease();
            pooledObject.Pool = _recyclableManager;
        }
        
        protected virtual Transform GetPoolRoot(IPoolable poolableObject) => poolableObject.IsUiElement ? _rootUi : _root;

    }
}