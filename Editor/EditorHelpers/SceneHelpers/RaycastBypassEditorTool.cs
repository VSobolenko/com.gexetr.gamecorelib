using System.Collections.Generic;
using Game;
using Game.Components.Utilities;
using Game.DynamicData;
using GameEditor.Internal;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor.SceneTools
{
[CustomEditor(typeof(RaycastBypassEditorUI))]
internal sealed class RaycastBypassEditorTool : Editor
{
    private readonly List<GameObject> _lasModifyGameObjects = new();
    private ReorderableList _reorderableList;

    [MenuItem(GameData.EditorName + EditorSubfolder.Scene + "/Create RaycastUI Handler")]
    public static void CreateSceneRaycastBypassObject()
    {
        var go = new GameObject("Raycast UI Handler", typeof(RaycastBypassEditorUI));
        if (InternalTools.IsPrefabStage(out var prefabRoot))
            go.transform.SetParent(prefabRoot, false);

        Undo.RegisterCreatedObjectUndo(go, "Create GameObject");
        Selection.activeGameObject = go;
    }

    private void OnEnable() => InitializeReorderableList();

    private void InitializeReorderableList()
    {
        _reorderableList = new ReorderableList(_lasModifyGameObjects, typeof(GameObject), true, false, false, false)
        {
            drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                GameObject gameObject = _lasModifyGameObjects[index];

                EditorGUI.ObjectField(rect, gameObject, typeof(GameObject), true);
            }
        };
    }

    public override void OnInspectorGUI()
    {
        var buttonStyle = GUILayout.Height(30);

        if (GUILayout.Button("Disable graphic raycast", buttonStyle))
            SetRaycastsFoundedByType<Graphic>(false);

        if (GUILayout.Button("Disable images raycast", buttonStyle))
            SetRaycastsFoundedByType<Image>(false);

        if (GUILayout.Button("Disable TextMeshProUGUI raycast", buttonStyle))
            SetRaycastsFoundedByType<TextMeshProUGUI>(false);

        if (GUILayout.Button("Enable graphic raycast", buttonStyle))
            SetRaycastsFoundedByType<TextMeshProUGUI>(true);

        // Not working vertical scroll
        // GUILayout.Label("Last Modified GameObjects:");
        // DrawReadonlyReorderableList(); 
    }

    private void SetRaycastsFoundedByType<T>(bool isRaycastTargets) where T : Graphic
    {
        var raycastComponents = InternalTools.IsPrefabStage(out var prefabRoot)
            ? prefabRoot.GetComponentsInChildren<T>(true)
            : FindObjectsOfType<T>(true);
        SetRaycastTargetComponents(raycastComponents, isRaycastTargets);
    }

    private void SetRaycastInChildren<T>(GameObject assignedGameObject, bool isRaycastTargets) where T : Graphic
    {
        var raycastComponents = assignedGameObject.GetComponentsInChildren<T>(true);
        SetRaycastTargetComponents(raycastComponents, isRaycastTargets);
    }

    private void SetRaycastTargetComponents<T>(IReadOnlyCollection<T> raycastComponents, bool isRaycastTargets)
        where T : Graphic
    {
        _lasModifyGameObjects.Clear();
        foreach (var component in raycastComponents)
        {
            if (component.raycastTarget != isRaycastTargets)
            {
                _lasModifyGameObjects.Add(component.gameObject);
                EditorUtility.SetDirty(component);
            }

            component.raycastTarget = isRaycastTargets;
        }

        Log.Info($"[Raycast Handler] Count Raycast Targets={raycastComponents.Count}; " +
                 $"Count Disabled={_lasModifyGameObjects.Count}");
        EditorUtility.SetDirty(this);
    }

    private void DrawReadonlyReorderableList() => _reorderableList.DoList(EditorGUILayout.GetControlRect());
}
}