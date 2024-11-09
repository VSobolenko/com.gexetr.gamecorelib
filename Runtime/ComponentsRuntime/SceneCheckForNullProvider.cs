using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Components
{
// Editor Only script-utility
public class SceneCheckForNullProvider : MonoBehaviour
{
    [Header("If empty check all"), SerializeField]
    private List<string> _accessible = new() {"Assembly-CSharp", "GameCoreLib"};

    [SerializeField] private bool _enableClassPath;
    [SerializeField] private List<MonoBehaviour> _monoBehaviour;

    private void Start() => throw new ArgumentException("Editor Only script-utility");

    [ContextMenu("Auto check")]
    public void AutoTest()
    {
        Collect();
        Process();
    }

    [ContextMenu("Collect scripts")]
    private void Collect() => _monoBehaviour = GetScripts().ToList();

    private IEnumerable<MonoBehaviour> GetScripts() => _accessible.Count > 0
        ? FindObjectsOfType<MonoBehaviour>()
            .Where(x => _accessible.Contains(x.GetType().Assembly.GetName().Name))
        : FindObjectsOfType<MonoBehaviour>();

    [ContextMenu("Check collected scripts")]
    private void Process() => _monoBehaviour.ForEach(
        x =>
        {
            if (x != null)
                CheckField(x, new List<object>(), x.gameObject, x.GetType().Name);
        });

    private void CheckField<T1>(T1 instance, List<object> ignored, GameObject parent, string parentField)
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance));

        if (ignored.Contains(instance))
            return;

        ignored.Add(instance);
        var fields = instance
                     .GetType()
                     .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var fieldInfo in fields)
        {
            if (fieldInfo.GetCustomAttributes(typeof(SerializeField), true).Any() == false)
                continue;
            var filedPath = $"{parentField}/{fieldInfo.Name}" +
                            (_enableClassPath ? $"({instance.GetType().Name})" : string.Empty);
            var text = $"GO={parent.name}; " +
                       $"Field={fieldInfo.Name}; " +
                       $"IsUnityObj={fieldInfo.GetValue(instance) is Object}" +
                       $"\nField path: {filedPath}" +
                       $"\nScene path: {GetScenePath(parent.transform)}";

            var systemObject = fieldInfo.GetValue(instance);
            if ((systemObject is Object unityObject && unityObject == null) || (systemObject == null))
                Debug.Log(text);
            else if ((fieldInfo.FieldType.IsClass || fieldInfo.FieldType.IsValueType) &&
                     _accessible.Contains(fieldInfo.FieldType.Assembly.GetName().Name))
                CheckField(systemObject, ignored, parent, filedPath);
        }
    }

    private static string GetScenePath(Transform obj) =>
        obj.parent == null ? obj.name : $"{GetScenePath(obj.parent)}/{obj.name}";
}
}