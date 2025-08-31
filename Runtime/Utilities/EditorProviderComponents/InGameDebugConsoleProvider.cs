using UnityEngine;

namespace Game.Components
{
/// <summary>
/// In game debug console to view console from build
/// Make sure that resources asset not include to release build
/// Move file outside from resources folder
/// </summary>
public sealed class InGameDebugConsoleProvider : MonoBehaviour
{
    [SerializeField] private bool _enableSelfDestruct = true;
    [SerializeField] private bool _enableInEditor;
    [SerializeField] private int _countTouchToDestroy = 5;

    private const string PrefabPath = "IngameDebugConsole";

    private GameObject _cachedConsole;

    private void Awake()
    {
        if (_enableSelfDestruct)
            Destroy(gameObject);
        else
            EnableInGameDebugConsole();
    }

    private void EnableInGameDebugConsole()
    {
        if (Application.isEditor == false)
            Log.Warning($"Turn on {nameof(InGameDebugConsoleProvider)}");

        if (Application.isEditor == false && _enableInEditor)
            Log.Errored($"Will be loaded {PrefabPath}.prefab; If this is a Dev_Build, you can skip it");

        if (_enableInEditor == false)
            return;

        var consolePrefab = Resources.Load<GameObject>(PrefabPath);
        if (consolePrefab == null)
        {
            Log.Warning($" On the path:\"{PrefabPath}\" prefab not found");

            return;
        }

        _cachedConsole = Instantiate(consolePrefab);
    }

    private void Update()
    {
        if (Input.touchCount == _countTouchToDestroy)
            Destroy(_cachedConsole.gameObject);
    }
}
}