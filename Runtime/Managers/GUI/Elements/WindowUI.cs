using Game.Extensions;
using UnityEngine;

namespace Game.GUI.Windows
{
[RequireComponent(typeof(CanvasGroup))]
public class WindowUI : MonoBehaviour
{
    [SerializeField] protected internal CanvasGroup canvasGroup;
    [SerializeField] protected internal RectTransform overrideTransition;
    [SerializeField] protected internal RectTransform overrideTabView;

    [ContextMenu("Validate")]
    private void Reset() => this.With(x => x.canvasGroup = GetComponent<CanvasGroup>(), canvasGroup == null);
}
}