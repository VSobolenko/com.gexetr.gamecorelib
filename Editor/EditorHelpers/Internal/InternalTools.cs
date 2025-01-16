using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameEditor.Internal
{
internal static class InternalTools
{
    public static bool IsPrefabStage(out Transform root)
    {
        root = null;
        var prefabContext = PrefabStageUtility.GetCurrentPrefabStage()?.prefabContentsRoot;

        if (prefabContext == null)
            return false;
        root = prefabContext.transform;

        return true;
    }
    
    public static bool CheckFieldForNull<T1>(T1 monoBehaviour, ICollection<object> ignored, GameObject parent, string parentField,
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
                       $"\nScene path: {GetObjectHierarchyPath(parent.transform)}";

            var systemObject = fieldInfo.GetValue(monoBehaviour);
            if (systemObject is Object unityObject && unityObject == null || (systemObject == null))
            {
                Log.Warning(text);
                healthy = false;
            }
            else if ((fieldInfo.FieldType.IsClass || fieldInfo.FieldType.IsValueType) &&
                     accessibleAssembly.Contains(fieldInfo.FieldType.Assembly.GetName().Name))
                healthy &= CheckFieldForNull(systemObject, ignored, parent, filedPath, enableClassPath, accessibleAssembly);
        }

        return healthy;
    }

    public static string GetObjectHierarchyPath(Transform t) =>
        t.parent == null ? t.name : $"{GetObjectHierarchyPath(t.parent)}/{t.name}";

    public static string GetObjectHierarchyPath(GameObject g) =>
        g.transform.parent == null ? g.name : $"{GetObjectHierarchyPath(g.transform.parent)}/{g.name}";
}
}