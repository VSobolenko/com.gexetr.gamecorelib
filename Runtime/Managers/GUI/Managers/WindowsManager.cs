using System;
using System.Collections.Generic;
using Game.GUI.Windows;
using Game.GUI.Windows.Factories;
using UnityEngine;

namespace Game.GUI.Managers
{
internal class WindowsManager : IWindowsManager
{
    protected readonly WindowConstructor<IMediator> constructor;

    public WindowsManager(IWindowFactory windowFactory, Transform rootUi)
    {
        if (windowFactory.TryCreateWindowsRoot(rootUi, out var root) == false)
            Log.Warning($"In {GetType().Name} empty root");

        constructor = new WindowConstructor<IMediator>(windowFactory, root);
    }

    public void Dispose()
    {
        constructor.Dispose();
    }

    #region Container

    public bool TryGetWindows<TMediator>(out TMediator[] mediator) where TMediator : class, IMediator
    {
        var mediators = new List<TMediator>();
        foreach (WindowData<IMediator> window in constructor)
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
        foreach (WindowData<IMediator> window in constructor)
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
        constructor.HideWindow(constructor.Count - 1, false);

        return constructor.OpenWindowSilently(initWindow).Mediator as TMediator;
    }

    public TMediator OpenWindowOver<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator
    {
        constructor.HideWindow(constructor.Count - 1, true);

        return constructor.OpenWindowSilently(initWindow).Mediator as TMediator;
    }

    public bool CloseWindow<TMediator>() where TMediator : class, IMediator
    {
        for (var i = 0; i < constructor.Count; i++)
        {
            if (constructor[i].Mediator.GetType() != typeof(TMediator))
                continue;

            constructor.CloseWindow(i);

            return true;
        }
        Log.Warning($"No {typeof(TMediator)} window found to close");
        return false;
    }

    public bool CloseWindow<TMediator>(TMediator mediator) where TMediator : class, IMediator
    {
        if (mediator == null)
            throw new ArgumentNullException($"Close {typeof(TMediator)} null mediator");

        for (var i = 0; i < constructor.Count; i++)
        {
            if (constructor[i].Mediator != mediator)
                continue;

            constructor.CloseWindow(i);

            return true;
        }
        Log.Warning($"No {typeof(TMediator).Name} window found to close");
        return false;
    }

    public void CloseWindows()
    {
        var countWindows = constructor.Count;
        for (var i = countWindows - 1; i >= 0; i--)
        {
            constructor.CloseWindow(i);
        }
    }

    #endregion
}
}