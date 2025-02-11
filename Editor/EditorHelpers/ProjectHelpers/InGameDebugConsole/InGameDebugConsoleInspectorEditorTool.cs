using Game.Components;
using UnityEditor;
using UnityEngine;

namespace GameEditor.ProjectTools
{
[CustomEditor(typeof(InGameDebugConsoleProvider))]
internal class InGameDebugConsoleInspectorEditorTool : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Move To Resources"))
            InGameDebugConsoleProviderEditorTool.MoveToResourcesStatic();
        if (GUILayout.Button("Select Prefab"))
            InGameDebugConsoleProviderEditorTool.SelectInGamePrefabStatic();
        if (GUILayout.Button("Move From Resources"))
            InGameDebugConsoleProviderEditorTool.MoveFromResourcesStatic();
    }
}
}