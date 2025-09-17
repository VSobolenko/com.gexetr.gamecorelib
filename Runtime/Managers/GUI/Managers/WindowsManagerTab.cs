using System;
using System.Collections.Generic;
using System.Linq;
using Game.GUI.Windows;
using Game.GUI.WindowsFactories;
using UnityEngine;

namespace Game.GUI.Managers
{
public interface ITabSwitcher<in T> where T : struct, Enum
{
    float Height { get; }
    void SetAsLastSibling();
    void OpenTabSmooth(T tab);
}

public class WindowsManagerTab<TSwitcher, TTabEnum> : IWindowsManager
    where TSwitcher : ITabSwitcher<TTabEnum> 
    where TTabEnum : struct, Enum
{
    private readonly WindowConstructorTab<IMediator, TSwitcher, TTabEnum> _constructor;
    private readonly IWindowFactory _windowFactory;
    private readonly Transform _root;

    public TSwitcher Switcher => _constructor.Switcher;

    public WindowsManagerTab(Transform root, IWindowFactory windowFactory,
                             MediatorToTabBinder<Type, TTabEnum> mediatorTabBinder)
    {
        _root = root;
        _windowFactory = windowFactory;

        _windowFactory.TryCreateTabSwitcher<TSwitcher, TTabEnum>(_root, out var switcher);
        _constructor = new WindowConstructorTab<IMediator, TSwitcher, TTabEnum>(switcher, mediatorTabBinder);
    }

    public void GenerateTabs()
    {
        var mediators = _constructor.TabBinder.Select(x => x.Key);
        foreach (var type in mediators)
        {
            if (_windowFactory.TryCreateWindow(type, _root, out var mediator, out var window) == false)
                throw new ArgumentNullException(type.Name, $"Can't create mediator {type}");
            _constructor.InsertTab(mediator, window);
        }
    }

    public bool TryGetWindows<TMediator>(out TMediator[] mediator) where TMediator : class, IMediator
    {
        var mediators = new List<TMediator>();
        foreach (WindowData<IMediator> window in _constructor)
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
        foreach (WindowData<IMediator> window in _constructor)
        {
            if (window.Mediator is TMediator == false)
                continue;
            mediator = (TMediator) window.Mediator;

            break;
        }

        return mediator != null;
    }

    public TMediator OpenWindow<TMediator>(Action<TMediator> initWindow = null, OpenMode mode = OpenMode.Overlay,
                                           int priority = 0) where TMediator : class, IMediator
    {
        if (_constructor.Has<TMediator>() == false)
            throw new ArgumentException($"{typeof(TMediator).Name} not found!");

        return _constructor.OpenWindowSilently<TMediator>().Mediator as TMediator;
    }

    public bool CloseWindow<TMediator>() where TMediator : class, IMediator => throw new NotSupportedException();

    public bool CloseWindow<TMediator>(TMediator mediator) where TMediator : class, IMediator => throw new NotSupportedException();

    public void CloseWindows() => throw new NotSupportedException();

    public void Dispose()
    {
        _constructor.Dispose();
    }
}
}