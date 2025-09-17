using UnityEngine;

namespace Game.AssetContent
{
public interface IResourceFactoryManager
{
    T CreateGameObjectWithComponent<T>(string key, Vector3 position, Quaternion quaternion, Transform parent = null) where T : Component;
    
    GameObject CreateGameObject(string key, Vector3 position, Quaternion quaternion, Transform parent = null);
}
}