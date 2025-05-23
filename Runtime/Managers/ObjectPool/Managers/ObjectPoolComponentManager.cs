﻿using System;
using System.Collections.Generic;
using Game.DynamicData;
using Game.Factories;
using UnityEngine;

namespace Game.Pools.Managers
{
/// <summary>
/// Object pool by component type
/// If the object type is already present, other prefabs but with the same type are ignored
/// </summary>
internal class ObjectPoolComponentManager : IComponentObjectPoolManager
{
    private readonly IFactoryGameObjects _factoryGameObjects;
    private readonly Dictionary<GameObject, IComponentObjectPool<Component>> _pool;
    private readonly Transform _root;
    private readonly ObjectPoolProfilerProvider _poolProfiler;
    protected int DefaultCapacity;

    public ObjectPoolComponentManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot,
        int capacity)
    {
        _factoryGameObjects = objectFactoryGameObjects;
        DefaultCapacity = Mathf.Max(0, capacity);
        _pool = new Dictionary<GameObject, IComponentObjectPool<Component>>(DefaultCapacity);

        _root = poolRoot;
        _poolProfiler = SetupEditorHierarchyStructureAndCreatePoolProfiler(poolRoot);
    }

    private ObjectPoolProfilerProvider SetupEditorHierarchyStructureAndCreatePoolProfiler(Transform poolRoot)
    {
        if (Application.isEditor == false)
            return null;

        _root.name = $"{GameData.Identifier}.Root";

        if (poolRoot.TryGetComponent<ObjectPoolProfiler>(out var profilerView)) //ToDo: testing this and is it necessary at all? 
            return new ObjectPoolProfilerProvider(poolRoot)
                .AssignProfilerView(profilerView)
                .Initialize(this, _pool);

        return new ObjectPoolProfilerProvider(poolRoot).Initialize(this, _pool);
    }

    public IComponentObjectPool<Component> Prepare<T>(T prefab, int count, bool force = false) where T : Component
    {
        var pool = Warn(prefab, count);
        var countExists = pool.Count;

        count = force ? count : count - countExists;
        for (var i = 0; i < count; i++)
            CreateOrReturnElementToPool(prefab, pool, false);
        _poolProfiler?.Update();
        return pool;
    }

    public T Get<T>(T prefab) where T : Component =>
        InternalGet(prefab, Vector3.zero, Quaternion.identity, null);

    public T Get<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component =>
        InternalGet(prefab, position, rotation, null);

    public T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true)
        where T : Component =>
        InternalGet(prefab, position, rotation, parent);

    public T Get<T>(T prefab, Transform parent, bool inWorldSpace = true) where T : Component =>
        InternalGet(prefab, Vector3.zero, Quaternion.identity, parent);

    public void Release<T>(T prefabInstance) where T : Component
    {
        if (prefabInstance == null)
            throw new ArgumentNullException(nameof(prefabInstance),
                $"Can't execute {nameof(Release)} with null {typeof(T).Name}");

        if (_pool.TryGetValue(prefabInstance.gameObject, out var pool) == false)
            throw new ArgumentException(
                $"Return unknown prefab to pool. Use {nameof(Prepare)} first. PrefabType={prefabInstance.GetType()}");

        pool.Release(prefabInstance);
        _poolProfiler?.Update();
    }

    private T InternalGet<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent)
        where T : Component
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab), $"Can't execute {nameof(Get)} with null {typeof(T).Name}");

        if (_pool.TryGetValue(prefab.gameObject, out var pool) == false)
            throw new ArgumentException($"An unknown object was requested. Use {nameof(Prepare)} first");

        var pooledObject = pool.Get(position, rotation, parent);
        _poolProfiler?.Update();

        return pooledObject as T;
    }

    protected virtual IComponentObjectPool<Component> Warn<T>(T prefab, int expectedCountNewElements)
        where T : Component
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab), $"Can't execute {nameof(Prepare)} with null {typeof(T).Name}");
        
        if (expectedCountNewElements < 0)
            throw new ArgumentOutOfRangeException(nameof(expectedCountNewElements),
                $"Expected count to be prepared can't be negative for {typeof(T).Name}.");
        
        if (_pool.TryGetValue(prefab.gameObject, out var existPool))
            return existPool;

        var root = GetPoolRoot(prefab);
        var pool = new ComponentObjectPool<Component>(expectedCountNewElements, root,
            () => _factoryGameObjects.Instantiate(prefab, root));
        
        if (_pool.Count > DefaultCapacity)
        {
            Log.Warning("Pool capacity exceeded. Use an increased size of the original container");
            DefaultCapacity = _pool.Count;
        }

        _pool.Add(prefab.gameObject, pool);
        _poolProfiler?.Update();
        return pool;
    }

    private void CreateOrReturnElementToPool<T>(T prefab, IComponentObjectPool<Component> pool, bool isInstance)
        where T : Component
    {
        var pooledObject = isInstance ? prefab : pool.CreateInstance();
        pool.Release(pooledObject);

        _poolProfiler?.Update();
    }

    protected virtual Transform GetPoolRoot(Component component) => _root;
}
}