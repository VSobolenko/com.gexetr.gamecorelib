using Game;
using Game.Localizations.Components;
using UnityEditor;
using UnityEngine;

namespace GameEditor.Localizations
{
[CustomEditor(typeof(LocalizableTMP))]
internal sealed class LocalizableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Editor localize"))
            Localize();
    }

    private void Localize()
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