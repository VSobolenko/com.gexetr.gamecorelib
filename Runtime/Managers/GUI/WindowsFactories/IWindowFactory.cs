using UnityEngine;

namespace Game.GUI.Windows.Factories
{
public interface IWindowFactory
{
    public bool TryCreateWindowsRoot(Transform root, out Transform uiRoot);

    public bool TryCreateWindow<TMediator>(Transform root, out TMediator mediator, out WindowUI window)
        where TMediator : class, IMediator;
}
}