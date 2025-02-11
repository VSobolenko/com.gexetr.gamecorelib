using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game;
using Game.Components.Utilities;
using GameEditor.Internal;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameEditor.Common
{
[CustomEditor(typeof(CheckForNullEditorProvider))]
internal class CheckForNullInspectorEditorProvider : Editor
{
    private CheckForNullEditorProvider Self => (CheckForNullEditorProvider)target;

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Collect scripts"))
            Self._monoBehaviour = GetScripts(Self._accessible).ToArray();
        if (GUILayout.Button("Check collected scripts"))
            ProcessCheckFieldForNull(Self._monoBehaviour, Self._accessible, Self._enableClassPath);
        base.OnInspectorGUI();
    }

    public static void ProcessCheckFieldForNull(string[] accessible, bool enableClassPath)
    {
        var healthy = true;
        var monoBehaviours = GetScripts(accessible);

        foreach (var x in monoBehaviours)
        {
            if (x == null)
            {
                Log.Info("Attempt to check null monoBehaviour. Check skipped for this object");
                continue;
            }

            healthy &= CheckField(x, new List<object>(), x.gameObject, x.GetType().Name, enableClassPath, accessible);
        }

        if (healthy) Log.Info("All objects are healthy");
    }

    public static void ProcessCheckFieldForNull(MonoBehaviour[] monoBehaviours, string[] accessible, bool enableClassPath)
    {
        var healthy = true;
        foreach (var x in monoBehaviours)
        {
            if (x == null)
            {
                Log.Info("Attempt to check null monoBehaviour. Check skipped for this object");
                continue;
            }

            healthy &= CheckField(x, new List<object>(), x.gameObject, x.GetType().Name, enableClassPath, accessible);
        }

        if (healthy) Log.Info("All objects are healthy");
    }

    public static MonoBehaviour[] GetScriptsInRoot(GameObject gameObject, string[] accessibleAssembly)
    {
        var monoBehaviours = gameObject.GetComponentsInChildren<MonoBehaviour>();

        if (accessibleAssembly.Length > 0)
            monoBehaviours = monoBehaviours.Where(x =>
                x != null && accessibleAssembly.Contains(x.GetType().Assembly.GetName().Name)).ToArray();

        return monoBehaviours;
    }

    private static MonoBehaviour[] GetScripts(string[] accessibleAssembly)
    {
        var allScripts = InternalTools.IsPrefabStage(out var root)
            ? root.GetComponentsInChildren<MonoBehaviour>()
            : FindObjectsOfType<MonoBehaviour>(true);

        if (accessibleAssembly.Length > 0)
            allScripts = allScripts.Where(x =>
                x != null && accessibleAssembly.Contains(x.GetType().Assembly.GetName().Name)).ToArray();

        return allScripts;
    }

    private static bool CheckField<T>(
        T monoBehaviour,
        ICollection<object> ignored,
        GameObject parent,
        string parentField,
        bool enableClassPath,
        string[] accessibleAssembly)
    {
        if (monoBehaviour == null)
            throw new ArgumentNullException(nameof(monoBehaviour));

        if (ignored.Contains(monoBehaviour))
            return true;

        ignored.Add(monoBehaviour);
        var healthy = true;
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
                healthy &= CheckField(systemObject, ignored, parent, filedPath, enableClassPath,
                    accessibleAssembly);
        }

        return healthy;
    }

    private static string GetScenePath(Transform obj) =>
        obj.parent == null ? obj.name : $"{GetScenePath(obj.parent)}/{obj.name}";
}
}