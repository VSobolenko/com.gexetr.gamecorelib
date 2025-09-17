using System;
using System.Collections.Generic;
using Game.Factories;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Pools.Managers
{
/// <summary>
/// Object pool by object type
/// If the object type is already present, other prefabs but with the same type are ignored
/// </summary>
internal class ObjectPoolTypeManager : IObjectPoolManager
{
    private readonly IFactoryGameObjects _factoryGameObjects;
    private readonly Dictionary<Type, IPoolableObjectPool<IPoolable>> _pool;
    private readonly Transform _root;
    private readonly Transform _rootUi;
    private readonly ObjectPoolProfilerProvider _poolProfiler;
    protected int DefaultCapacity;

    public ObjectPoolTypeManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot, int capacity)
    {
        _factoryGameObjects = objectFactoryGameObjects;
        DefaultCapacity = Mathf.Max(0, capacity);
        _pool = new Dictionary<Type, IPoolableObjectPool<IPoolable>>(DefaultCapacity);

        _root = CreateObjectPoolRoot(poolRoot);
        _rootUi = CreateAndSetupUIObjectPoolRoot(poolRoot);
        _poolProfiler = SetupEditorHierarchyStructureAndCreatePoolProfiler(poolRoot);
    }

    protected Transform CreateObjectPoolRoot(Transform poolRoot) =>
        _factoryGameObjects.InstantiateEmpty(poolRoot).transform;

    protected Transform CreateAndSetupUIObjectPoolRoot(Transform poolRoot)
    {
        var rootUI = _factoryGameObjects
                     .InstantiateEmpty(poolRoot, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler),
                                       typeof(GraphicRaycaster)).transform;

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
        InternalGet(prefab, position, rotation, parent);

    public T Get<T>(T prefab, Transform parent, bool inWorldSpace = true) where T : Component, IPoolable =>
        InternalGet(prefab, Vector3.zero, Quaternion.identity, parent);

    public void Release<T>(T prefabInstance) where T : Component, IPoolable
    {
        if (prefabInstance == null)
            throw new ArgumentNullException(nameof(prefabInstance),
                $"Can't execute {nameof(Release)} with null {typeof(T).Name}");
        
        if (_pool.TryGetValue(prefabInstance.GetType(), out var pool) == false)
            throw new ArgumentException(
                $"Return unknown prefab to pool. Use {nameof(Prepare)} first. Prefab={prefabInstance.GetType().Name}");

        pool.Release(prefabInstance);
        _poolProfiler?.Update();
    }

    private T InternalGet<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent)
        where T : IPoolable
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab), $"Can't execute {nameof(Get)} with null {typeof(T).Name}");

        if (_pool.TryGetValue(prefab.GetType(), out var pool) == false)
            throw new ArgumentException($"An unknown object was requested. Use {nameof(Prepare)} first");
        
        var pooledObject = pool.Get(position, rotation, parent);
        _poolProfiler?.Update();

        return (T) pooledObject;
    }

    protected virtual IPoolableObjectPool<IPoolable> Warn<T>(T prefab, int expectedCountNewElements) where T : Component, IPoolable
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab), $"Can't execute {nameof(Prepare)} with null {typeof(T).Name}");

        if (expectedCountNewElements < 0)
            throw new ArgumentOutOfRangeException(nameof(expectedCountNewElements),
                $"Expected count to be prepared can't be negative for {typeof(T).Name}.");
        
        if (_pool.TryGetValue(prefab.GetType(), out var existPool))
            return existPool;

        var root = GetPoolRoot(prefab);
        var pool = new PoolableObjectPool<IPoolable>(expectedCountNewElements, root,
            () => _factoryGameObjects.Instantiate(prefab, root));

        if (_pool.Count > DefaultCapacity)
        {
            Log.Warning("Pool capacity exceeded. Use an increased size of the original container");
            DefaultCapacity = _pool.Count;
        }

        _pool.Add(prefab.GetType(), pool);
        _poolProfiler?.Update();
        return pool;
    }

    private void CreateOrReturnElementToPool<T>(T prefab, IPoolableObjectPool<T> pool, bool isInstance)
        where T : class, IPoolable
    {
        var pooledObject = isInstance ? prefab : pool.CreateInstance();

        pool.Release(pooledObject);

        _poolProfiler?.Update();
    }
    
    protected virtual Transform GetPoolRoot(IPoolable poolableObject) => poolableObject.IsUiElement ? _rootUi : _root;
}
}