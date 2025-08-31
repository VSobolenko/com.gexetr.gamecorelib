using System.Collections.Generic;
using Game.Factories;
using UnityEngine;

namespace Game.Pools.Managers
{
/// <summary>
/// Object pool by object type
/// Same as "ObjectPoolTypeManager"
/// Added division in the hierarchy for easy testing
/// </summary>
internal sealed class ObjectPoolTypeEditorSeparateManager : ObjectPoolTypeManager
{
    private readonly Dictionary<string, Transform> _pool;

    public ObjectPoolTypeEditorSeparateManager(IFactoryGameObjects objectFactoryGameObjects, Transform poolRoot, int capacity)
        : base(objectFactoryGameObjects, poolRoot, capacity)
    {
        _pool = new Dictionary<string, Transform>(DefaultCapacity);
    }
    
    protected override IPoolableObjectPool<IPoolable> Warn<T>(T prefab, int expectedCountNewElements)
    {
        if (_pool.ContainsKey(prefab.Key)) 
            return base.Warn(prefab, expectedCountNewElements);
        
        var root = base.GetPoolRoot(prefab);
        var parent = prefab.IsUiElement ? CreateAndSetupUIObjectPoolRoot(root) : CreateObjectPoolRoot(root);
        if (Application.isEditor)
            parent.name = $"[{_pool.Count}] {prefab.GetType().Name}";
        _pool.Add(prefab.Key, parent);

        return base.Warn(prefab, expectedCountNewElements);
    }

    protected override Transform GetPoolRoot(IPoolable poolableObject) => _pool[poolableObject.Key];
}
}