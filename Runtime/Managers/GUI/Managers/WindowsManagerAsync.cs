using System;
using System.Threading.Tasks;
using Game.GUI.Windows.Factories;
using Game.GUI.Windows.Transitions;
using UnityEngine;

namespace Game.GUI.Windows.Managers
{
internal class WindowsManagerAsync : WindowsManager, IWindowsManagerAsync
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
        var closingWindow = windowBuilder.Count > 0 ? windowBuilder[^1] : default;
        var openingWindow = windowBuilder.OpenWindowSilently(initWindow);
        
        var closeTask = closingWindow?.mediator != null ? transition.Close(closingWindow) : Task.CompletedTask;
        var openTask = transition.Open(openingWindow);

        closingWindow?.mediator?.SetInteraction(false);
        openingWindow.mediator?.SetInteraction(false);
        
        await Task.WhenAll(closeTask, openTask);

        openingWindow.mediator?.SetInteraction(true);
        
        return openingWindow.mediator as TMediator;
    }

    public async Task<bool> CloseWindowAsync<TMediator>() where TMediator : class, IMediator
    {
        for (var i = 0; i < windowBuilder.Count; i++)
        {
            if (windowBuilder[i].mediator.GetType() != typeof(TMediator))
                continue;

            var closingWindows = windowBuilder[i];
            WindowProperties openingWindow = null;
            if (i == windowBuilder.Count - 1 && i != 0)
                openingWindow = windowBuilder[i - 1];
            else
                openingWindow = null;
            
            var result = await CloseWindowAsync(closingWindows, openingWindow, i, _defaultCloseTransition);

            return result;
        }

        return false;
    }

    // DANGER - похожий метод, посмотреть
    public async Task<bool> CloseWindowAsync<TMediator>(TMediator mediator) where TMediator : class, IMediator
    {
        for (var i = 0; i < windowBuilder.Count; i++)
        {
            if (windowBuilder[i].mediator != mediator)
                continue;

            var closingWindows = windowBuilder[i];
            var openingWindow = windowBuilder.Count == 1 ? default : windowBuilder[i - 1];
            var result = await CloseWindowAsync(closingWindows, openingWindow, i, _defaultCloseTransition);

            return result;
        }

        return false;
    }

    private async Task<bool> CloseWindowAsync(WindowProperties closingWindow, WindowProperties openingWindow,
                                              int closingWindowIndex, IWindowTransition transition)
    {
        var closeTask = transition.Close(closingWindow);
        var openTask = openingWindow.mediator != null ? transition.Open(openingWindow) : Task.CompletedTask;

        closingWindow.mediator.SetInteraction(false);
        openingWindow.mediator?.SetInteraction(false);
        
        await Task.WhenAll(closeTask, openTask);
        
        openingWindow.mediator?.SetInteraction(true);

        windowBuilder.CloseWindow(closingWindowIndex);

        return true;
    }
}
}