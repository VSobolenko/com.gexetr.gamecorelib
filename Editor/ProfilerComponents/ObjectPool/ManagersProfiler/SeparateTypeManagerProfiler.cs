﻿using System;
using System.Collections.Generic;
using Game.Pools;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace GameEditor.Pools
{
internal class SeparateTypeManagerProfiler : IPoolProfiler
{
    private readonly Type _poolType;
    private readonly  Dictionary<Type, ObjectPool<IPoolable>> _pool;

    public SeparateTypeManagerProfiler(object pool, Type poolType)
    {
        _poolType = poolType;
        _pool = pool as  Dictionary<Type, ObjectPool<IPoolable>>;
        if (_pool == null)
            Debug.LogError($"Can't unboxing pool dictionary for {GetType().Name} profiler");
    }

    public void DrawStatus(VisualElement root)
    {
        if (_pool == null)
            return;
        
        var infoStyle = new GUIStyle(GUI.skin.box);
        
        GUILayout.Label($"[Types] Pool: {_poolType.Name}; Profiler: {GetType().Name}", infoStyle);
        GUILayout.Label($"Pool capacity: {_pool.Keys.Count}");
    }
    
    public void OnPoolDataUpdated() { }
}
}