using Game.Extensions;
using UnityEngine;

namespace Game.Components
{
[System.Flags]
public enum SafeAreaEdge
{
    None   = 0,
    Left   = 1 << 0,
    Right  = 1 << 1,
    Top    = 1 << 2,
    Bottom = 1 << 3,
    All    = Left | Right | Top | Bottom
}

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class SafeScreenArea : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransformComponent;
    [SerializeField] private SafeAreaEdge initialEdge = SafeAreaEdge.All;
    [SerializeField] private bool initialEnable = true;

    public RectTransform RectTransform => rectTransformComponent;
    
    private void Awake()
    {
        if (initialEnable)
            RecalculateRectTransform(initialEdge);
    }

    public void RecalculateRectTransform(SafeAreaEdge edges)
    {
        var safeArea = Screen.safeArea;

        var minX = safeArea.x / Screen.width;
        var maxX = (safeArea.x + safeArea.width) / Screen.width;
        var minY = safeArea.y / Screen.height;
        var maxY = (safeArea.y + safeArea.height) / Screen.height;

        var anchorMin = rectTransformComponent.anchorMin;
        var anchorMax = rectTransformComponent.anchorMax;

        if ((edges & SafeAreaEdge.Left) != 0)
            anchorMin.x = minX;
        if ((edges & SafeAreaEdge.Right) != 0)
            anchorMax.x = maxX;
        if ((edges & SafeAreaEdge.Bottom) != 0)
            anchorMin.y = minY;
        if ((edges & SafeAreaEdge.Top) != 0)
            anchorMax.y = maxY;

        rectTransformComponent.anchorMin = anchorMin;
        rectTransformComponent.anchorMax = anchorMax;
    }
    
    [ContextMenu("Force Reset")]
    private void Reset() => this.With(x => x.rectTransformComponent = GetComponent<RectTransform>(), rectTransformComponent == null);
}
}