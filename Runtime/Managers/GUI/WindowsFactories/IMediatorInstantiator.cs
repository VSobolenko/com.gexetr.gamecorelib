using System;
using Game.GUI.Windows;

namespace Game.GUI.WindowsFactories
{
public interface IMediatorInstantiator
{
    TMediator Instantiate<TMediator>(WindowUI windowUI, params object[] extraArgs) where TMediator : class, IMediator;
    IMediator Instantiate(Type mediatorType, WindowUI windowUI, params object[] extraArgs);
}
}