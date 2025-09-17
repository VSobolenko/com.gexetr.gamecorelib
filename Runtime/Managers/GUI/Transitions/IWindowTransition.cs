using System.Threading.Tasks;
using Game.GUI.Managers;
using Game.GUI.Windows;
using UnityEngine;

namespace Game.GUI.Transitions
{
public interface IWindowTransition
{
    Task Open(WindowData<IMediator> windowData);
    Task Close(WindowData<IMediator> windowData);
}

internal static class WindowTransitionStatic
{
    public static Vector3 startPoint = Vector3.zero;
}
}