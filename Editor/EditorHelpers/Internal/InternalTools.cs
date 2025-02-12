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

    public static string GetObjectHierarchyPath(Transform t) =>
        t.parent == null ? t.name : $"{GetObjectHierarchyPath(t.parent)}/{t.name}";

    public static string GetObjectHierarchyPath(GameObject g) =>
        g.transform.parent == null ? g.name : $"{GetObjectHierarchyPath(g.transform.parent)}/{g.name}";
}
}