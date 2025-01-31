using Game.Pools;
using UnityEngine;

namespace GameTests.Tests.Runtime.ObjectPoolTests
{
internal class PoolableTestObject : IPoolable
{
    public string Key { get; }
    public bool IsUiElement { get; }

    public PoolableTestObject(string key, bool isUiElement)
    {
        Key = key;
        IsUiElement = isUiElement;
    }

    public IObjectPoolRecyclable Pool { get; set; }
    public void SetParent(Transform parent) { }
    public void SetPositionAndRotation(Vector3 position, Quaternion rotation) { }
    public void SetActive(bool status) { }
    public void OnUse() { }
    public void OnRelease() { }
}
}