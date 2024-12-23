using System;
using Game.Factories;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Pools.Managers
{
    internal class TypeObjectPool<T> : IPrefabObjectPool<T> where T : Component, IPoolable
    {
        private readonly T _prefab;
        private readonly Transform _root;
        private readonly Transform _rootUi;
        private readonly IObjectPool<IPoolable> _pool;
        private readonly IFactoryGameObjects _factoryGameObjects;
        private IObjectPoolRecyclable _recyclableManager;

        public TypeObjectPool(T prefab, IObjectPoolRecyclable recyclableManager, int expectedCountNewElements)
        {
            _recyclableManager = recyclableManager;
            _prefab = prefab;
            // _pool = new ObjectPool<IPoolable>(() => CreateNewElement(prefab, GetPoolRoot(prefab)),
            //     OnGetFromPool, 
            //     OnReturnedToPool, 
            //     poolable => Log.InternalError(), true,
            //     expectedCountNewElements);
        }

        public T Get() => InternalGet(Vector3.zero, Quaternion.identity, null);

        public T Get(Vector3 position, Quaternion rotation) =>  InternalGet(position, rotation, null);

        public T Get(Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true) => InternalGet(position, rotation, parent);

        public T Get(Transform parent, bool inWorldSpace = true) => InternalGet(Vector3.zero, Quaternion.identity, parent);

        public void Release(T prefabInstance)
        {
            if (prefabInstance == null)
                throw new ArgumentException($"Can't release null prefab");
            
            var parent = GetPoolRoot(prefabInstance);
            prefabInstance.SetParent(parent);
            prefabInstance.SetPositionAndRotation(Vector3.zero, Quaternion.identity);

            _pool.Release(prefabInstance);
        }
        
        private T InternalGet(Vector3 position, Quaternion rotation, Transform parent)
        {
            var pooledObject = (T) _pool.Get();

            pooledObject.SetParent(parent);
            pooledObject.SetPositionAndRotation(position, rotation);
            return pooledObject;
        }
        
        private IPoolable CreateNewElement(T prefab, Transform parent)
        {
            var pooledObject = _factoryGameObjects.Instantiate(prefab, parent);
            pooledObject.SetActive(false);
            pooledObject.Pool = _recyclableManager;
            if (Application.isEditor)
                pooledObject.name += pooledObject.GetHashCode();
            return pooledObject;
        }

        private static void OnGetFromPool(T pooledObject)
        {
            pooledObject.SetActive(true);
            pooledObject.OnUse();
            throw new NotImplementedException("Not implement generics");
        }

        private static void OnReturnedToPool(T pooledObject)
        {
            pooledObject.SetActive(false);
            pooledObject.OnRelease();
            throw new NotImplementedException("Not implement generics");
        }
        
        protected virtual Transform GetPoolRoot(IPoolable poolableObject) => poolableObject.IsUiElement ? _rootUi : _root;
    }
}