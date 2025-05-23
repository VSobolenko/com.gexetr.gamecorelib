﻿using System.Collections.Generic;
using Game.Factories;
using UnityEngine;

namespace Game.Pools.Managers
{
/// <summary>
/// Object pool by poolable key
/// Same as "ObjectPoolKeyManager"
/// Added division in the hierarchy for easy testing
/// </summary>
internal class ObjectPoolKeyEditorSeparateManager : ObjectPoolKeyManager
{
    private readonly Dictionary<string, Transform> _pool;

    public ObjectPoolKeyEditorSeparateManager(IFactoryGameObjects objectFactoryGameObjects,
                                              Transform poolRoot,
                                              int capacity)
        : base(objectFactoryGameObjects, poolRoot, capacity)
    {
        _pool = new Dictionary<string, Transform>(DefaultCapacity);
    }

    protected override IPoolableObjectPool<IPoolable> Warn<T>(T prefab, int expectedCountNewElements)
    {
        if (_pool.ContainsKey(prefab.Key)) 
            return base.Warn(prefab, expectedCountNewElements);
        
        var root = base.GetPoolRoot<T>(prefab);
        var parent = prefab.IsUiElement ? CreateAndSetupUIObjectPoolRoot(root) : CreateObjectPoolRoot(root);
        if (Application.isEditor)
            parent.name = $"[{_pool.Count}] {prefab.Key}";
        _pool.Add(prefab.Key, parent);

        return base.Warn(prefab, expectedCountNewElements);

    }

    protected override Transform GetPoolRoot<T>(IPoolable poolableObject) => _pool[poolableObject.Key];
}
}