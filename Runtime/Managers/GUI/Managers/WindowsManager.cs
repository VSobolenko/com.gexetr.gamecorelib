using System;
using System.Collections.Generic;
using Game.GUI.Windows.Factories;
using UnityEngine;

namespace Game.GUI.Windows.Managers
{
internal class WindowsManager : IWindowsManager
{
    protected readonly WindowConstructor<IMediator> WindowConstructor;

    public WindowsManager(IWindowFactory windowFactory, Transform rootUi)
    {
        if (windowFactory.TryCreateWindowsRoot(rootUi, out var root) == false)
            Log.Warning($"In {GetType().Name} empty root");

        WindowConstructor = new WindowConstructor<IMediator>(windowFactory, root);
    }

    public void Dispose()
    {
        WindowConstructor.Dispose();
    }

    #region Container

    public bool TryGetWindows<TMediator>(out TMediator[] mediator) where TMediator : class, IMediator
    {
        var mediators = new List<TMediator>();
        foreach (WindowData<IMediator> window in WindowConstructor)
        {
            if (window.Mediator is TMediator == false)
                continue;
            mediators.Add((TMediator) window.Mediator);
        }

        mediator = mediators.ToArray();

        return mediator.Length > 0;
    }

    public bool TryGetFirstWindow<TMediator>(out TMediator mediator) where TMediator : class, IMediator
    {
        mediator = null;
        foreach (WindowData<IMediator> window in WindowConstructor)
        {
            if (window.Mediator is TMediator == false)
                continue;
            mediator = (TMediator) window.Mediator;

            break;
        }

        return mediator != null;
    }

    #endregion

    #region Static transition

    public TMediator OpenWindow<TMediator>(Action<TMediator> initWindow = null, OpenMode mode = OpenMode.Overlay, int priority = 0)
        where TMediator : class, IMediator
    {
        WindowConstructor.HideWindow(WindowConstructor.Count - 1, false);

        return WindowConstructor.OpenWindowSilently(initWindow).Mediator as TMediator;
    }

    public TMediator OpenWindowOver<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator
    {
        WindowConstructor.HideWindow(WindowConstructor.Count - 1, true);

        return WindowConstructor.OpenWindowSilently(initWindow).Mediator as TMediator;
    }

    public bool CloseWindow<TMediator>() where TMediator : class, IMediator
    {
        for (var i = 0; i < WindowConstructor.Count; i++)
        {
            if (WindowConstructor[i].Mediator.GetType() != typeof(TMediator))
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
            if (WindowConstructor[i].Mediator != mediator)
                continue;

            WindowConstructor.CloseWindow(i);

            return true;
        }
        Log.Warning($"No {typeof(TMediator).Name} window found to close");
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