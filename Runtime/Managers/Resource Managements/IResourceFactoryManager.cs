using UnityEngine;

namespace Game.AssetContent
{
public interface IResourceFactoryManager
{
    T CreateGameObjectWithComponent<T>(string key, Vector3 position, Quaternion quaternion, Transform parent) where T : Component;
}
}