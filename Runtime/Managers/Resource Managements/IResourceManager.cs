using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.AssetContent
{
    public interface IResourceManager
{
    T LoadAsset<T>(string key) where T : Object;

    Task<T>  LoadAssetAsync<T>(string key) where T : Object;

    Task LoadSceneAsync(string key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true);

    void ClearMemory();
}
}