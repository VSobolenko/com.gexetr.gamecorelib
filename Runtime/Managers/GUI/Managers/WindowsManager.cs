using System;
using System.Collections.Generic;
using Game.GUI.Windows.Factories;
using UnityEngine;

namespace Game.GUI.Windows.Managers
{
internal class WindowsManager : IWindowsManager
{
    protected readonly WindowConstructor WindowConstructor;

    public WindowsManager(IWindowFactory windowFactory, Transform rootUi)
    {
        if (windowFactory.TryCreateWindowsRoot(rootUi, out var root) == false)
            Log.Warning($"In {GetType().Name} empty root");

        WindowConstructor = new WindowConstructor(windowFactory, root);
    }

    public void Dispose()
    {
        WindowConstructor.Dispose();
    }

    #region Container

    public bool TryGetActiveWindows<TMediator>(out TMediator[] mediator) where TMediator : class, IMediator
    {
        var mediators = new List<TMediator>();
        foreach (WindowProperties window in WindowConstructor)
        {
            if (window.mediator is TMediator == false)
                continue;
            mediators.Add((TMediator) window.mediator);
        }

        mediator = mediators.ToArray();

        return mediator.Length > 0;
    }

    public bool TryGetActiveWindow<TMediator>(out TMediator mediator) where TMediator : class, IMediator
    {
        mediator = null;
        foreach (WindowProperties window in WindowConstructor)
        {
            if (window.mediator is TMediator == false)
                continue;
            mediator = (TMediator) window.mediator;

            break;
        }

        return mediator != null;
    }

    #endregion

    #region Static transition

    public TMediator OpenWindowOnTop<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator
    {
        WindowConstructor.HideWindow(WindowConstructor.Count - 1, false);

        return WindowConstructor.OpenWindowSilently(initWindow).mediator as TMediator;
    }

    public TMediator OpenWindowOver<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator
    {
        WindowConstructor.HideWindow(WindowConstructor.Count - 1, true);

        return WindowConstructor.OpenWindowSilently(initWindow).mediator as TMediator;
    }

    public bool CloseWindow<TMediator>() where TMediator : class, IMediator
    {
        for (var i = 0; i < WindowConstructor.Count; i++)
        {
            if (WindowConstructor[i].mediator.GetType() != typeof(TMediator))
                continue;

            WindowConstructor.CloseWindow(i);

            return true;
        }
        Log.Warning($"No {typeof(TMediator)} window found to close");
        return false;
    }

    public bool CloseWindow<TMediator>(TMediator mediator) where TMediator : class, IMediator
    {
        if (mediator == null)
            throw new ArgumentNullException($"Close {typeof(TMediator)} null mediator");

        for (var i = 0; i < WindowConstructor.Count; i++)
        {
            if (WindowConstructor[i].mediator != mediator)
                continue;

            WindowConstructor.CloseWindow(i);

            return true;
        }
        Log.Warning($"No {typeof(TMediator)} window found to close");
        return false;
    }

    public void CloseWindows()
    {
        var countWindows = WindowConstructor.Count;
        for (var i = countWindows - 1; i >= 0; i--)
        {
            WindowConstructor.CloseWindow(i);
        }
    }

    #endregion
}
}