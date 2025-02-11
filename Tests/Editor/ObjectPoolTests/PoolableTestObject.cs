using Game.Pools;
using UnityEngine;

namespace Game.Tests.Editor.ObjectPoolTests
{
internal class PoolableTestObject : IPoolable
{
    private string _key;
    private bool _isUI;
    
    public string Key => _key;
    public bool IsUiElement => _isUI;

    public PoolableTestObject(string key, bool isUiElement)
    {
        _key = key;
        _isUI = isUiElement;
    }

    public void SetParent(Transform parent) { }
    public void SetPositionAndRotation(Vector3 position, Quaternion rotation) { }
    public void SetActive(bool status) { }
    public void OnUse() { }
    public void OnRelease() { }
}
}