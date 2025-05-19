using System;

namespace Game.GUI.Windows
{
public enum OpenMode : byte
{
    Overlay = 0,
    Silently = 1,
}

public interface IWindowsContainer : IDisposable
{
    bool TryGetWindows<TMediator>(out TMediator[] mediator) where TMediator : class, IMediator;

    bool TryGetFirstWindow<TMediator>(out TMediator mediator) where TMediator : class, IMediator;
}

public interface IWindowsManager : IWindowsContainer
{
    TMediator OpenWindow<TMediator>(Action<TMediator> initWindow = null, OpenMode mode = OpenMode.Overlay, int priority = 0) 
        where TMediator : class, IMediator;
    
    bool CloseWindow<TMediator>() where TMediator : class, IMediator;

    bool CloseWindow<TMediator>(TMediator mediator) where TMediator : class, IMediator;

    void CloseWindows();
}

}