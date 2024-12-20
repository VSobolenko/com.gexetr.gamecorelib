using UnityEngine;

namespace Game.Pools
{
public class MonoPooledObject : BasePooledObject
{
    [SerializeField] private string key;

    public override string Key => key;

    [ContextMenu("Force start auto validate")]
    private void OnValidate()
    {
        if (string.IsNullOrEmpty(key))
            key = $"{GetType().Name}.{name}";
    }
}
}