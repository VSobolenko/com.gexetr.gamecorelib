using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.Pools;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEditor.Pools
{
internal sealed class KeyManagerProfiler : IPoolProfiler
{
    private readonly Type _poolType;
    private static int _maxPoolCapacity;
    private static int _maxPoolStackCapacity;
    private readonly Dictionary<string, IPoolableObjectPool<IPoolable>> _pool;
    private Dictionary<string, PoolableData> _poolData = new();

    public KeyManagerProfiler(object pool, Type poolType)
    {
        _poolType = poolType;
        _pool = pool as Dictionary<string, IPoolableObjectPool<IPoolable>>;
        if (_pool == null)
        {
            Log.Error($"Can't unboxing pool dictionary for {GetType().Name} profiler");

            return;
        }
    }

    public void DrawStatus(VisualElement root)
    {
        if (_pool == null)
            return;

        var infoStyle = new GUIStyle(GUI.skin.box);
        var greenStyle = new GUIStyle(GUI.skin.textArea) { normal = {textColor = new Color(0.49f, 1f, 0f)}, };
        var greenStyle2 = new GUIStyle(GUI.skin.textArea) { normal = {textColor = new Color(0.7f, 1f, 0.75f)}, };

        OnPoolDataUpdated();

        var stackCapacity = _pool.Values.Sum(stackValue => stackValue.Count);

        if (GUILayout.Button("Clear pool"))
            ClearPool();
        
        GUILayout.Label($"Pool: {_poolType.Name}\nProfiler: {GetType().Name}", infoStyle);
        GUILayout.Label($"Pool capacity: {_pool.Keys.Count}\nMax capacity: {_maxPoolCapacity}", greenStyle);

        GUILayout.Label($"Full stack capacity: {stackCapacity}\nMax stack capacity: {_maxPoolStackCapacity}",
                        greenStyle2);
        DrawPoolInfo();
    }

    public void OnPoolDataUpdated()
    {
        if (_pool == null)
            return;

        if (_pool.Keys.Count > _maxPoolCapacity)
            _maxPoolCapacity = _pool.Keys.Count;

        var stackCapacity = _pool.Values.Sum(stackValue => stackValue.Count);
        if (stackCapacity > _maxPoolStackCapacity)
            _maxPoolStackCapacity = stackCapacity;
        RecalculateData();
    }

    private void DrawPoolInfo()
    {
        var redStyle = new GUIStyle(GUI.skin.box)
        {
            normal = {textColor = Color.red},
        };

        var containerStyles = new List<GUIStyle>
        {
            new() {normal = {textColor = Color.white}},
            new() {normal = {textColor = Color.yellow}},
            new() {normal = {textColor = Color.cyan}},
            new() {normal = {textColor = Color.magenta}},
        };

        GUILayout.Label("Pool container", redStyle);
        for (var i = 0; i < _pool.Count; i++)
        {
            var (key, value) = _pool.ElementAt(i);

            var maxElements = _poolData.TryGetValue(key, out var poolableData) ? poolableData.maxElemets.ToString() : "-";

            var data = $"{Verify(key, value)}Count/Max: {value.Count}/{maxElements} \tKey: {key}";

            var style = containerStyles[i % containerStyles.Count];
            GUILayout.Label(data, style);
        }
    }

    private string Verify(string key, IPoolableObjectPool<IPoolable> stack)
    {
        // var existsCopy = stack.Count == stack.Distinct().Count(); //ToDo: verify
        // var hasNoNull = stack.Count(x => x == null) == 0;
        //
        // return existsCopy || hasNoNull ? "✓" : "x";
        return "✓";
    }

    private void ClearPool() => _pool.Clear();

    private void RecalculateData()
    {
        if (_pool == null)
        {
            Log.Error("Null pool");

            return;
        }

        var newPoolData = new Dictionary<string, PoolableData>(_pool.Count);

        foreach (var (key, stack) in _pool)
        {
            var poolData = new PoolableData
            {
                maxElemets = stack.Count,
            };

            newPoolData.Add(key, poolData);

            if (_poolData.TryGetValue(key, out var data))
                poolData.maxElemets = Mathf.Max(data.maxElemets, stack.Count);

            newPoolData[key] = poolData;
        }

        _poolData = newPoolData;
    }

    private struct PoolableData
    {
        public int maxElemets;
    }
}
}