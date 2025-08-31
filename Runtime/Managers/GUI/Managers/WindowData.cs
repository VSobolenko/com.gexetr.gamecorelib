using UnityEngine;

namespace Game.GUI.Windows.Managers
{
public sealed class WindowData<T>
{
    public T Mediator;
    public RectTransform RectTransform;
    public RectTransform Motor;
    public RectTransform Tab;
    public CanvasGroup CanvasGroup;
    public OpenMode Mode;
}
}