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

    public Task Open(WindowData windowData)
    {
        if (_openNormalize)
            NormalizeWindow(windowData);

        return _open?.Open(windowData);
    }

    public Task Close(WindowData windowData)
    {
        if (_closeNormalize)
            NormalizeWindow(windowData);

        return _close?.Close(windowData);
    }

    private void NormalizeWindow(WindowData windowData)
    {
        windowData.Mediator.SetPosition(WindowTransitionStatic.startPoint);
        windowData.Motor.localScale = Vector3.one;
    }
}
}