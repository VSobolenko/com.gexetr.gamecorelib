using System.Threading.Tasks;
using Game.GUI.Windows.Managers;
using UnityEngine;

namespace Game.GUI.Windows.Transitions
{
internal class ConfigurableTransition : IWindowTransition
{
    private readonly IWindowTransition _open;
    private readonly IWindowTransition _close;
    private readonly bool _openNormalize;
    private readonly bool _closeNormalize;

    public ConfigurableTransition(IWindowTransition open, IWindowTransition close, 
                                  bool openNormalize, bool closeNormalize)
    {
        _open = open;
        _close = close;
        _openNormalize = openNormalize;
        _closeNormalize = closeNormalize;
    }

    public Task Open(WindowProperties windowProperties)
    {
        if (_openNormalize)
            NormalizeWindow(windowProperties);

        return _open?.Open(windowProperties);
    }

    public Task Close(WindowProperties windowProperties)
    {
        if (_closeNormalize)
            NormalizeWindow(windowProperties);

        return _close?.Close(windowProperties);
    }

    private void NormalizeWindow(WindowProperties windowProperties)
    {
        windowProperties.mediator.SetPosition(WindowTransitionStatic.startPoint);
        windowProperties.motor.localScale = Vector3.one;
    }
}
}