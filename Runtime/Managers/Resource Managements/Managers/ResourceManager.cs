using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.AssetContent.Managers
{
internal sealed class ResourceManager : IResourceManager
{
    public T LoadAsset<T>(string key) where T : Object => Resources.Load<T>(key);

    public Task<T> LoadAssetAsync<T>(string key) where T : Object
    {
        var resourceRequest = Resources.LoadAsync<T>(key);
        var taskSource = new TaskCompletionSource<T>();
        resourceRequest.completed += _ =>
        {
            taskSource.SetResult(resourceRequest.asset as T);
        };

        return taskSource.Task;
    }

    public async Task LoadSceneAsync(string key, LoadSceneMode loadMode = LoadSceneMode.Single, bool activateOnLoad = true)
    {
        var request = SceneManager.LoadSceneAsync(key, loadMode);
        var taskSource = new TaskCompletionSource<bool>();
        request.completed += _ =>
        {
            taskSource.SetResult(true);
        };

        await taskSource.Task;
    }

    public void ClearMemory()
    {
        throw new System.NotImplementedException();
    }
}
}