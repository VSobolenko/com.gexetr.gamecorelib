using System;

namespace Game.GUI.Windows.Factories
{
public interface IMediatorInstantiator
{
    TMediator Instantiate<TMediator>(WindowUI windowUI) where TMediator : class, IMediator;
    IMediator Instantiate(Type mediatorType, WindowUI windowUI);
}
}