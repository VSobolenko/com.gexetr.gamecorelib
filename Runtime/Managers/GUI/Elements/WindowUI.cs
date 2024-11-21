using UnityEngine;

namespace Game.GUI.Windows
{
[DefaultExecutionOrder(11), RequireComponent(typeof(CanvasGroup))]
public class WindowUI : MonoBehaviour
{
    [SerializeField] protected internal CanvasGroup canvasGroup;
    [SerializeField] protected internal RectTransform overrideTransition;

    [ContextMenu("Validate")]
    private void Reset()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }
}
}