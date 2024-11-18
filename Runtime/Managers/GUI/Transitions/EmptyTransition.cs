using System.Threading.Tasks;
using Game.GUI.Windows.Managers;
using UnityEngine;

namespace Game.GUI.Windows.Transitions
{
public class EmptyTransition : IWindowTransition
{
    public Task Open(WindowProperties windowProperties)
    {
        windowProperties.motor.localPosition = WindowTransitionStatic.startPoint;
        windowProperties.motor.localScale = Vector3.one;
        return Task.CompletedTask;
    }

    public Task Close(WindowProperties windowProperties)
    {
        windowProperties.motor.localPosition = WindowTransitionStatic.startPoint;
        windowProperties.motor.localScale = Vector3.one;
        return Task.CompletedTask;
    }
}
}