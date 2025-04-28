using System;
using System.Collections.Generic;
using Game;
using Game.Pools;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEditor.Pools
{
internal class SeparateKeyManagerProfiler : IPoolProfiler
{
    private readonly Type _poolType;
    private readonly Dictionary<string, Stack<IPoolable>> _pool;

    public SeparateKeyManagerProfiler(object pool, Type poolType)
    {
        _poolType = poolType;
        _pool = pool as Dictionary<string, Stack<IPoolable>>;
        if (_pool == null)
            Log.Errored($"Can't unboxing pool dictionary for {GetType().Name} profiler");
    }

    public void DrawStatus(VisualElement root)
    {
        if (_pool == null)
            return;
        
        var infoStyle = new GUIStyle(GUI.skin.box);
        
        GUILayout.Label($"Pool: {_poolType.Name}\nProfiler: {GetType().Name}", infoStyle);
        GUILayout.Label($"Pool capacity: {_pool.Keys.Count}");
    }
    
    public void OnPoolDataUpdated() { }
}
}