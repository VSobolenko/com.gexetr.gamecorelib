using Game.Extensions;
using UnityEngine;

namespace Game.GUI.Windows
{
[DefaultExecutionOrder(11), RequireComponent(typeof(CanvasGroup))]
public class WindowUI : MonoBehaviour
{
    [SerializeField, HideInInspector] protected internal CanvasGroup canvasGroup;
    [SerializeField] protected internal RectTransform overrideTransition;

    [ContextMenu("Validate")]
    private void Reset() => this.With(x => x.canvasGroup = GetComponent<CanvasGroup>(), canvasGroup == null);
}
}