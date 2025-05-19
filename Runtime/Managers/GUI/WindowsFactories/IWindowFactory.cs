using System;
using Game.GUI.Windows.Managers;
using UnityEngine;

namespace Game.GUI.Windows.Factories
{
public interface IWindowFactory
{
    public bool TryCreateWindowsRoot(Transform root, out Transform uiRoot);

    public bool TryCreateWindow<TMediator>(Transform root, out TMediator mediator, out WindowUI window)
        where TMediator : class, IMediator;

    public bool TryCreateWindow(Type mediatorType, Transform root, out IMediator mediator, out WindowUI window);
    public bool TryCreateTabSwitcher<T>(Transform root, out ITabSwitcher<T> switcher) where T : struct, Enum;
}
}