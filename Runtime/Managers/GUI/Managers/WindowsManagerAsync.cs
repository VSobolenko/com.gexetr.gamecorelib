using System;
using System.Threading.Tasks;
using Game.GUI.Components;
using Game.GUI.Transitions;
using Game.GUI.Windows;
using Game.GUI.Windows.Factories;
using UnityEngine;

namespace Game.GUI.Managers
{
internal sealed class WindowsManagerAsync : WindowsManager, IWindowsManagerAsync
{
    private readonly IWindowTransition _defaultOpenTransition;
    private readonly IWindowTransition _defaultCloseTransition;

    public WindowsManagerAsync(IWindowFactory windowFactory, Transform rootUi, WindowSettings settings,
                               IWindowTransition openTransition, IWindowTransition closeTransition) : base(windowFactory, rootUi)
    {
        _defaultOpenTransition = openTransition;
        _defaultCloseTransition = closeTransition;
    }

    public async Task<TMediator> OpenWindowOnTopAsync<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator => await OpenWindowAsync(_defaultOpenTransition, false, initWindow);

    public async Task<TMediator> OpenWindowOverAsync<TMediator>(Action<TMediator> initWindow = null)
        where TMediator : class, IMediator => await OpenWindowAsync(_defaultOpenTransition, true, initWindow);

    public async Task<TMediator> OpenWindowOnTopAsync<TMediator>(IWindowTransition transition = null,
                                                                 Action<TMediator> initWindow = null)
        where TMediator : class, IMediator => await OpenWindowAsync(transition, false, initWindow);

    public async Task<TMediator> OpenWindowOverAsync<TMediator>(IWindowTransition transition = null,
                                                                Action<TMediator> initWindow = null)
        where TMediator : class, IMediator => await OpenWindowAsync(transition, true, initWindow);

    private async Task<TMediator> OpenWindowAsync<TMediator>(IWindowTransition transition, bool deactivateLastWindow,
                                                             Action<TMediator> initWindow)
        where TMediator : class, IMediator
    {
        var closingWindow = constructor.Count > 0 ? constructor[^1] : default;
        var openingWindow = constructor.OpenWindowSilently(initWindow);
        
        var closeTask = closingWindow?.Mediator != null ? transition.Close(closingWindow) : Task.CompletedTask;
        var openTask = transition.Open(openingWindow);

        closingWindow?.Mediator?.SetInteraction(false);
        openingWindow.Mediator?.SetInteraction(false);
        
        await Task.WhenAll(closeTask, openTask);

        openingWindow.Mediator?.SetInteraction(true);
        
        return openingWindow.Mediator as TMediator;
    }

    public async Task<bool> CloseWindowAsync<TMediator>() where TMediator : class, IMediator =>
        await CloseWindowAsync<TMediator>(_defaultCloseTransition);

    public async Task<bool> CloseWindowAsync<TMediator>(IWindowTransition transition) where TMediator : class, IMediator
    {
        for (var i = 0; i < constructor.Count; i++)
        {
            if (constructor[i].Mediator.GetType() != typeof(TMediator))
                continue;

            var closingWindows = constructor[i];
            WindowData<IMediator> openingWindow = null;
            if (i == constructor.Count - 1 && i != 0)
                openingWindow = constructor[i - 1];
            
            var result = await CloseWindowAsync(closingWindows, openingWindow, i, transition);

            return result;
        }

        return false;
    }
        

    public async Task<bool> CloseWindowAsync<TMediator>(TMediator mediator) where TMediator : class, IMediator =>
        await CloseWindowAsync(_defaultCloseTransition, mediator);

    public async Task<bool> CloseWindowAsync<TMediator>(IWindowTransition transition, TMediator mediator)
        where TMediator : class, IMediator
    {
        for (var i = 0; i < constructor.Count; i++)
        {
            if (constructor[i].Mediator != mediator)
                continue;

            var closingWindows = constructor[i];
            var openingWindow = constructor.Count == 1 ? default : constructor[i - 1];
            var result = await CloseWindowAsync(closingWindows, openingWindow, i, transition);

            return result;
        }

        return false;
    }

    private async Task<bool> CloseWindowAsync(WindowData<IMediator> closingWindow, WindowData<IMediator> openingWindow,
                                              int closingWindowIndex, IWindowTransition transition)
    {
        var closeTask = transition.Close(closingWindow);
        var openTask = openingWindow.Mediator != null ? transition.Open(openingWindow) : Task.CompletedTask;

        closingWindow.Mediator.SetInteraction(false);
        openingWindow.Mediator?.SetInteraction(false);
        
        await Task.WhenAll(closeTask, openTask);
        
        openingWindow.Mediator?.SetInteraction(true);

        constructor.CloseWindow(closingWindowIndex);

        return true;
    }
}
}