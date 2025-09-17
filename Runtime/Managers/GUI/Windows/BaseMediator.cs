using UnityEngine;

namespace Game.GUI.Windows
{
/// <summary>
/// Base class for window mediators
/// Method execution order:
/// +--------------------------------+
/// | SetActive()
/// |
/// | OnInitialize()
/// |  OnFocus()
/// |  InitAction.Invoke()
/// |  ...
/// |    SetActive()
/// |    OnFocus() / OnUnfocused()
/// |  ...
/// |  OnUnfocused()
/// |  OnDestroy()
/// |
/// |  Destroy()
/// +--------------------------------+
/// </summary>
/// <typeparam name="TWindow"></typeparam>
public abstract class BaseMediator<TWindow> : IMediator where TWindow : WindowUI
{
    protected readonly TWindow window;

    protected BaseMediator(TWindow window)
    {
        this.window = window;
    }

    public virtual void OnInitialize() { }
    public virtual void OnFocus() { }
    public virtual void OnUnfocused() { }
    public virtual void OnDestroy() { }

    public virtual void SetActive(bool value)
    {
        if (IsUnSafeExecution())
            return;
        
        window.gameObject.SetActive(value);
    }

    public virtual void SetPosition(Vector3 value)
    {
        if (IsUnSafeExecution())
            return;
        
        window.transform.localPosition = value;
    }

    public virtual void SetInteraction(bool value)
    {
        if (IsUnSafeExecution())
            return;
        
        window.config.canvasGroup.blocksRaycasts = value;
    }

    public virtual bool IsActive()
    {
        if (IsUnSafeExecution())
            return default;
        
        return window.gameObject.activeInHierarchy;
    }

    public virtual void Destroy()
    {
        if (IsUnSafeExecution())
            return;

        Object.Destroy(window.gameObject);
    }

    // Exception error when Unity itself deletes GameObject before us when exiting Play Mode
    private bool IsUnSafeExecution()
    {
        var windowIsMissing = window == null;
        if (windowIsMissing)
            Log.Warner($"It is not possible to execute with an empty {GetType().Name}. Ignore this when exiting Play mode.");

        return windowIsMissing;
    }
}
}