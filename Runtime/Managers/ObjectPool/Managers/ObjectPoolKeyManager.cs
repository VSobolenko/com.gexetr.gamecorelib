using System;
using System.Collections.Generic;
using Game.Factories;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pools.Managers
{
/// <summary>
/// Object pool by key type
/// Identical objects with the same type are represented as different if the keys are different
/// </summary>
internal class ObjectPoolKeyManager : IObjectPoolManager
{
    private readonly Dictionary<string, IPoolableObjectPool<IPoolable>> _pool;
    private readonly Transform _root;
    private readonly Transform _rootUi;
    private readonly ObjectPoolProfilerProvider _poolProfiler;
    private readonly IFactoryGameObjects _factoryGameObjects;
    protected int DefaultCapacity;
    
    public ObjectPoolKeyManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot, int capacity)
    {
        _factoryGameObjects = objectFactoryGameObjects;
        DefaultCapacity = Mathf.Max(0, capacity);
        _pool = new Dictionary<string, IPoolableObjectPool<IPoolable>>(DefaultCapacity);

        _root = CreateObjectPoolRoot(poolRoot);
        _rootUi = CreateAndSetupUIObjectPoolRoot(poolRoot);
        _poolProfiler = SetupEditorHierarchyStructureAndCreatePoolProfiler(poolRoot);
    }

    protected Transform CreateObjectPoolRoot(Transform poolRoot) =>
        _factoryGameObjects.InstantiateEmpty(poolRoot).transform;

    protected Transform CreateAndSetupUIObjectPoolRoot(Transform poolRoot)
    {
        var rootUI = _factoryGameObjects
                     .InstantiateEmpty(poolRoot, typeof(RectTransform), typeof(Canvas), 
                         typeof(CanvasScaler), typeof(GraphicRaycaster))
                     .transform;

        var canvas = rootUI.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.vertexColorAlwaysGammaSpace = true;

        return rootUI;
    }

    private ObjectPoolProfilerProvider SetupEditorHierarchyStructureAndCreatePoolProfiler(Transform poolRoot)
    {
        if (Application.isEditor == false)
            return null;

        poolRoot.name = $"{GameData.Identifier}.Pool";
        _root.name = $"{GameData.Identifier}.Root";
        _rootUi.name = $"{GameData.Identifier}.RootUI";

        if (poolRoot.TryGetComponent<ObjectPoolProfiler>(out var profilerView))
            return new ObjectPoolProfilerProvider(poolRoot)
                   .AssignProfilerView(profilerView)
                   .Initialize(this, _pool);

        return new ObjectPoolProfilerProvider(poolRoot).Initialize(this, _pool);
    }

    //ToDo: use fluent interface design and return (out) IPoolableObjectPool<IPoolable> as input parametr
    public IPoolableObjectPool<IPoolable> Prepare<T>(T prefab, int count, bool force = false) where T : Component, IPoolable
    {
        var pool = Warn(prefab, count);
        var countExists = pool.Count;
        
        count = force ? count : count - countExists;
        for (var i = 0; i < count; i++)
            CreateOrReturnElementToPool(prefab, pool, false);
        _poolProfiler?.Update();
        return pool;
    }

    public T Get<T>(T prefab) where T : Component, IPoolable =>
        InternalGet(prefab, Vector3.zero, Quaternion.identity, null);

    public T Get<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component, IPoolable =>
        InternalGet(prefab, position, rotation, null);

    public T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true) where T : Component, IPoolable =>
        InternalGet(prefab, position, rotation, parent, inWorldSpace);

    public T Get<T>(T prefab, Transform parent, bool inWorldSpace = true) where T : Component, IPoolable =>
        InternalGet(prefab, Vector3.zero, Quaternion.identity, parent, inWorldSpace);

    public void Release<T>(T prefabInstance) where T : Component, IPoolable
    {
        if (prefabInstance == null)
            throw new ArgumentNullException(nameof(prefabInstance),
                $"Can't execute {nameof(Release)} with null {typeof(T).Name}");

        if (_pool.TryGetValue(prefabInstance.Key, out var pool) == false)
            throw new ArgumentException(
                $"Return unknown prefab to pool. Use {nameof(Prepare)} first. Prefab={prefabInstance.Key}");

        CreateOrReturnElementToPool(prefabInstance, pool, true);
    }
    
    private T InternalGet<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent, bool inWorldSpace = true)
        where T : Component, IPoolable
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab), $"Can't execute {nameof(Get)} with null {typeof(T).Name}");

        if (_pool.TryGetValue(prefab.Key, out var pool) == false)
            throw new ArgumentException($"An unknown object was requested. Use {nameof(Prepare)} first");

        if (pool.Count == 0)
            CreateOrReturnElementToPool<T>(prefab, pool, false);
        
        var pooledObject = _pool[prefab.Key].Get(position, rotation, parent, inWorldSpace);
        _poolProfiler?.Update();

        return (T) pooledObject;
    }
    
    private void CreateOrReturnElementToPool<T>(T prefab, IObjectPool<IPoolable> pool, bool isInstance)
        where T : class, IPoolable
    {
        var pooledObject = isInstance ? prefab : pool.CreateInstance();
        pool.Release(pooledObject);

        _poolProfiler?.Update();
    }
    
    protected virtual IPoolableObjectPool<IPoolable> Warn<T>(T prefab, int expectedCountNewElements) where T : Component, IPoolable
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab), $"Can't execute {nameof(Prepare)} with null {typeof(T).Name}");

        if (prefab.Key == null)
            throw new ArgumentNullException(nameof(prefab.Key), $"Added Null or Empty key to Pool. Prefab name \"{prefab.name}\"");
        
        if (expectedCountNewElements < 0)
            throw new ArgumentOutOfRangeException(nameof(expectedCountNewElements),
                $"Expected count to be prepared can't be negative for {typeof(T).Name}.");

        if (_pool.TryGetValue(prefab.Key, out var existPool))
            return existPool;

        if (prefab.Key == null)
            Log.Warning($"Added Null or Empty key to Pool. Prefab name \"{prefab.name}\"");
        
        if (_pool.Keys.Count > DefaultCapacity)
        {
            Log.Warning("Pool capacity exceeded. Use an increased size of the original container");
            DefaultCapacity = _pool.Count;
        }

        var root = GetPoolRoot<T>(prefab);
        //ToDo: inverse control of _factoryGameObjects.Instantiate using Func<P,T,G>
        var pool = new PoolableObjectPool<IPoolable>(expectedCountNewElements, root, () =>
        {
            var element = _factoryGameObjects.Instantiate(prefab, root);
            return element;
        });
        _pool.Add(prefab.Key, pool);
        return pool;
    }

    protected virtual Transform GetPoolRoot<T>(IPoolable poolableObject) where T : class => poolableObject.IsUiElement ? _rootUi : _root;
}
}