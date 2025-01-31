using System;
using UnityEngine;

namespace Game.Pools
{
    public class GameObjectObjectPool<T> : ObjectPool<T>, IGameObjectObjectPool<T> where T : Component
    {
        private readonly Transform _root;

        public GameObjectObjectPool(int capacity, Transform root, Func<T> createInstance) : base(capacity, createInstance)
        {
            _root = root;
        }

        public override void Release(T instance) => InternalRelease(instance);

        public override T Get() => InternalGet(Vector3.zero, Quaternion.identity, null, true);

        public T Get(Vector3 position, Quaternion rotation) => InternalGet(position, rotation, null, true);

        public T Get(Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true) =>
            InternalGet(position, rotation, parent, inWorldSpace);

        public T Get(Transform parent, bool inWorldSpace = true) =>
            InternalGet(Vector3.zero, Quaternion.identity, parent, inWorldSpace);

        private T InternalGet(Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace)
        {
            var pooledObject = Pool.Count != 0 ? Pool.Dequeue() : CreateInstance();
        
            if (inWorldSpace)
            {
                pooledObject.transform.SetPositionAndRotation(position, rotation);
                pooledObject.transform.SetParent(parent);
            }   
            else
            {
                pooledObject.transform.SetParent(parent);
                pooledObject.transform.SetLocalPositionAndRotation(position, rotation);
            }

            pooledObject.gameObject.SetActive(true);

            return pooledObject;
        }

        private void InternalRelease(T instance)
        {
            if (Pool.Contains(instance))
                throw new InvalidOperationException($"The element \"{instance.GetType().Name}\" is already in the pool!");

            Pool.Enqueue(instance);
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_root);
        }
    }
}