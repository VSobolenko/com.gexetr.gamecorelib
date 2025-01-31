using System.Collections.Generic;
using Game.Factories;
using UnityEngine;

namespace Game.Pools.Managers
{
    internal class ObjectPoolGameObjectSeparateManager : ObjectPoolGameObjectManager
    {
        private readonly Dictionary<GameObject, Transform> _pool;

        public ObjectPoolGameObjectSeparateManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot, int capacity)
            : base(objectFactoryGameObjects, poolRoot, capacity)
        {
            _pool = new Dictionary<GameObject, Transform>(DefaultCapacity);
        }
    
        protected override IGameObjectObjectPool<Component> Warn<T>(T prefab, int expectedCountNewElements)
        {
            var pool = base.Warn(prefab, expectedCountNewElements);

            if (_pool.ContainsKey(prefab.gameObject))
                return pool;
            var parent = base.GetPoolRoot(prefab);
            if (Application.isEditor)
                parent.name = $"[{_pool.Count}] {prefab.GetType().Name}";
            _pool.Add(prefab.gameObject, parent);
            return pool;
        }

        protected override Transform GetPoolRoot(Component component) => _pool[component.gameObject];
    }
}