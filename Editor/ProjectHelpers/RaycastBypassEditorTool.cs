using Game;
using Game.Components.Utilities;
using Game.DynamicData;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GameEditor.Tools
{
[CustomEditor(typeof(RaycastBypassEditorUI))]
public class RaycastBypassEditorTools : Editor
{
    [MenuItem(GameData.EditorName + "/Create RaycastUI Handler")]
    public static void CreateSceneRaycastBypassObject()
    {
        var go = new GameObject("Raycast UI Handler", typeof(RaycastBypassEditorUI));
        Selection.activeGameObject = go;
    }

    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Disable graphic raycast in children") && target is RaycastBypassEditorUI raycastGraphic)
            DisableRaycastInChildren<Graphic>(raycastGraphic.gameObject);
        GUILayout.Space(10);

        if (GUILayout.Button("Disable images raycast in children") && target is RaycastBypassEditorUI raycastImage)
            DisableRaycastInChildren<Image>(raycastImage.gameObject);
        GUILayout.Space(10);

        if (GUILayout.Button("Disable TextMeshProUGUI raycast in children") &&
            target is RaycastBypassEditorUI raycastTextMeshProUGUI)
            DisableRaycastInChildren<TextMeshProUGUI>(raycastTextMeshProUGUI.gameObject);
        GUILayout.Space(10);

        if (GUILayout.Button("Disable All Graphic raycast in scene/prefab"))
            DisableRaycastFindByType<Graphic>();
        GUILayout.Space(10);
    }

    private void DisableRaycastFindByType<T>() where T : Graphic
    {
        var raycastComponents = FindObjectsOfType<T>(true);
        DisableRaycast(raycastComponents);
    }

    private void DisableRaycastInChildren<T>(GameObject assignedGameObject) where T : Graphic
    {
        var raycastComponents = assignedGameObject.GetComponentsInChildren<T>(true);
        DisableRaycast(raycastComponents);
    }

    private void DisableRaycast<T>(T[] raycastComponents) where T : Graphic
    {
        var countDisable = 0;
        foreach (var component in raycastComponents)
        {
            if (component.raycastTarget)
                countDisable++;
            component.raycastTarget = false;
        }

        Log.Info($"Disable raycast: Count Raycast Objects={raycastComponents.Length}; Count Disabled={countDisable}");
        EditorUtility.SetDirty(this);
    }
}
}