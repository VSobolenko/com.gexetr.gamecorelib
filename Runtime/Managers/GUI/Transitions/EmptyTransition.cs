using System.Threading.Tasks;
using Game.GUI.Windows.Managers;
using UnityEngine;

namespace Game.GUI.Windows.Transitions
{
public sealed class EmptyTransition : IWindowTransition
{
    public Task Open(WindowData<IMediator> windowData)
    {
        windowData.Motor.localPosition = WindowTransitionStatic.startPoint;
        windowData.Motor.localScale = Vector3.one;
        return Task.CompletedTask;
    }

    public Task Close(WindowData<IMediator> windowData)
    {
        windowData.Motor.localPosition = WindowTransitionStatic.startPoint;
        windowData.Motor.localScale = Vector3.one;
        return Task.CompletedTask;
    }
}
}