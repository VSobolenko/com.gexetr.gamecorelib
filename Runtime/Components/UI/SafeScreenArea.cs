﻿using Game.Extensions;
using UnityEngine;

namespace Game.Components
{
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
internal class SafeScreenArea : MonoBehaviour
{
    [SerializeField, HideInInspector] private RectTransform rectTransformComponent;
    
    private void Awake()
    {
        RecalculateRectTransform();
    }

    public void RecalculateRectTransform()
    {
        var safeArea = Screen.safeArea;
        var anchorMin = safeArea.position;
        var anchorMax = anchorMin + safeArea.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        rectTransformComponent.anchorMin = anchorMin;
        rectTransformComponent.anchorMax = anchorMax;
    }
     
    [ContextMenu("Force Reset")]
    private void Reset() => this.With(x => x.rectTransformComponent = GetComponent<RectTransform>(), rectTransformComponent == null);
}
}