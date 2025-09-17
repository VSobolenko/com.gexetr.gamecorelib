using Game.GUI.Windows;
using UnityEngine;

namespace Game.GUI.Managers
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