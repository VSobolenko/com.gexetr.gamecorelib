using UnityEngine;

namespace Game.Pools
{
public class MonoPooledObject : BasePooledObject
{
    [SerializeField] private string key;

    public override string Key => key;

    [ContextMenu("Force start auto validate")]
    private void OnValidate() => ValidatePoolKey(ref key);

    protected virtual void ValidatePoolKey(ref string poolKey)
    {
        if (string.IsNullOrEmpty(poolKey))
            poolKey = $"{GetType().Name}.{name}";
    }
}
}