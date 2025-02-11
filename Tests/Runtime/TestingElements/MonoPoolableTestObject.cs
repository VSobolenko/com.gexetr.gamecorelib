using Game.Pools;
using UnityEngine;

namespace Game.Tests.Runtime.TestingElements
{
internal class MonoPoolableTestObject : MonoBehaviour, IPoolable
{
    [SerializeField, HideInInspector] private string _key;
    [SerializeField, HideInInspector] private bool _isUI;

    public string Key
    {
        get => _key;
        set => _key = value;
    }

    public bool IsUiElement
    {
        get => _isUI;
        set => _isUI = value;
    }

    public void SetParent(Transform parent) { }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation) { }

    public void SetActive(bool status) { }

    public void OnUse() { }

    public void OnRelease() { }
}
}