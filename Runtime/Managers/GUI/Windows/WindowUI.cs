using System;
using Game.Extensions;
using UnityEngine;

namespace Game.GUI.Windows
{
[RequireComponent(typeof(CanvasGroup))]
public abstract class WindowUI : MonoBehaviour
{
    [SerializeField] protected internal WindowConfig config;

    [ContextMenu("Validate")]
    protected virtual void Reset()
    {
        if (config == null)
            config = new WindowConfig();

        this.With(x => x.config.canvasGroup = GetComponent<CanvasGroup>(), config.canvasGroup == null);
    }

    [Serializable]
    protected internal class WindowConfig
    {
        [SerializeField] protected internal CanvasGroup canvasGroup;
        [SerializeField] protected internal RectTransform overrideTransition;
        [SerializeField] protected internal RectTransform overrideTabView;
    }
}
}