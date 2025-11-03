using System;
using Game.GUI.Managers;
using Game.GUI.Windows;
using UnityEngine;

namespace Game.GUI.Windows.Factories
{
public interface IWindowFactory
{
    bool TryCreateWindowsRoot(Transform root, out Transform uiRoot);

    bool TryCreateWindow<TMediator>(Transform root, out TMediator mediator, out WindowUI window)
        where TMediator : class, IMediator;

    bool TryCreateWindow(Type mediatorType, Transform root, out IMediator mediator, out WindowUI window);

    bool TryCreateTabSwitcher<TSwitcher, TEnum>(Transform root, out TSwitcher switcher)
        where TSwitcher : ITabSwitcher<TEnum> 
        where TEnum : struct, Enum;
}
}