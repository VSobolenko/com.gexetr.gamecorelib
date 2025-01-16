using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Components.Utilities
{
/// <summary>
/// Class which provide inspector functionality
/// </summary>
public class SceneCheckForNullEditorProvider : MonoBehaviour
{
    [Header("If empty check all"), SerializeField]
    internal List<string> _accessible = new() {"Assembly-CSharp", "GameCoreLib"};

    [SerializeField] private bool _enableClassPath;
    [SerializeField] private List<MonoBehaviour> _monoBehaviour;

    private void Start() => throw new ArgumentException($"\"{name}\" has editor only \"{GetType().Name}\" component.");

    [ContextMenu("Collect scripts")]
    private void Collect() => _monoBehaviour = GetScripts(_accessible).ToList();

    [ContextMenu("Check collected scripts")]
    private void Process()
    {
        var healthy = true;
        foreach (var x in _monoBehaviour)
        {
            if (x == null)
                continue;
            healthy &= CheckField(x, new List<object>(), x.gameObject,
                                  x.GetType().Name, _enableClassPath, _accessible);
        }

        if (healthy)
            Log.Info("All objects are healthy");
    }

    public static IEnumerable<MonoBehaviour> GetScripts(List<string> accessibleAssembly) => accessibleAssembly.Count > 0
        ? FindObjectsOfType<MonoBehaviour>()
            .Where(x => x != null && accessibleAssembly.Contains(x.GetType().Assembly.GetName().Name))
        : FindObjectsOfType<MonoBehaviour>(); //ToDo: Working on prefabs ????

    public static bool CheckField<T1>(T1 monoBehaviour, ICollection<object> ignored, GameObject parent, string parentField,
                                      bool enableClassPath, List<string> accessibleAssembly)
    {
        var healthy = true;
        if (monoBehaviour == null)
            throw new ArgumentNullException(nameof(monoBehaviour));

        if (ignored.Contains(monoBehaviour))
            return true;

        ignored.Add(monoBehaviour);
        var fields = monoBehaviour
                     .GetType()
                     .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var fieldInfo in fields)
        {
            if (fieldInfo.GetCustomAttributes(typeof(SerializeField), true).Any() == false)
                continue;
            var filedPath = $"{parentField}/{fieldInfo.Name}" +
                            (enableClassPath ? $"({monoBehaviour.GetType().Name})" : string.Empty);
            var text = $"GO={parent.name}; " +
                       $"Field={fieldInfo.Name}; " +
                       $"IsUnityObj={fieldInfo.GetValue(monoBehaviour) is Object}" +
                       $"\nField path: {filedPath}" +
                       $"\nScene path: {GetScenePath(parent.transform)}";

            var systemObject = fieldInfo.GetValue(monoBehaviour);
            if (systemObject is Object unityObject && unityObject == null || (systemObject == null))
            {
                Log.Warning(text);
                healthy = false;
            }
            else if ((fieldInfo.FieldType.IsClass || fieldInfo.FieldType.IsValueType) &&
                     accessibleAssembly.Contains(fieldInfo.FieldType.Assembly.GetName().Name))
                healthy &= CheckField(systemObject, ignored, parent, filedPath, enableClassPath, accessibleAssembly);
        }

        return healthy;
    }

    private static string GetScenePath(Transform obj) =>
        obj.parent == null ? obj.name : $"{GetScenePath(obj.parent)}/{obj.name}";

    public void MarkVerifiedGameObjects(List<MonoBehaviour> verifyGameObjectsToNull)
    {
        
    }
}
}