using Game;
using Game.Localizations.Components;
using UnityEditor;
using UnityEngine;

namespace GameEditor.Localizations
{
[CustomEditor(typeof(LocalizableTMP))]
internal class LocalizableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Editor localize"))
        {
            var localizableBehaviour = (LocalizableTMP) target;
            if (localizableBehaviour == null)
            {
                Log.InternalError();
                return;
            }
            localizableBehaviour.EditorLocalize();
        }
    }
}
}