using System.Threading.Tasks;
using Game.GUI.Windows.Managers;
using UnityEngine;

namespace Game.GUI.Windows.Transitions
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